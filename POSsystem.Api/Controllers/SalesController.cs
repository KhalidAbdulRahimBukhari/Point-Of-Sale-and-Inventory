using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSsystem.Api.DTOs;
using POSsystem.Api.Models;

[ApiController]
[Route("api/sales")]
[Authorize(Roles = "Admin,Cashier")]
public class SaleController : ControllerBase
{
    private readonly PosDbContext _context;

    private const decimal TAX_PERCENT = 15m;
    private const int SALE_MOVEMENT_TYPE = 1;
    private const string STATUS_COMPLETED = "Completed";
    private const string STOCK_REASON_SALE = "Sale";

    public SaleController(PosDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSale([FromBody] SaleDraftDto draft)
    {
        if (draft == null || draft.Items == null || !draft.Items.Any())
            return BadRequest("Invalid sale data.");

        using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            var now = DateTime.UtcNow;

            var variantIds = draft.Items
                .Select(i => i.VariantId)
                .Distinct()
                .ToList();

            var variants = await _context.ProductVariants
                .Where(v => variantIds.Contains(v.VariantId))
                .ToDictionaryAsync(v => v.VariantId);

            if (variants.Count != variantIds.Count)
                return BadRequest("Inconsistant product variant.");

            foreach (var item in draft.Items)
            {
                if (variants[item.VariantId].CurrentStock < item.Qty)
                    return BadRequest($"Insufficient stock for {item.Name}");
            }

            var serverSubTotal = draft.Items.Sum(i =>
                variants[i.VariantId].SellingPrice * i.Qty);

            var serverTax = serverSubTotal * TAX_PERCENT / 100;

            var discountPercent = Math.Min(draft.Totals.DiscountPercentage, 15);
            var serverDiscount = serverSubTotal * discountPercent / 100;

            var serverGrandTotal = serverSubTotal + serverTax - serverDiscount;

            if (Math.Abs(serverGrandTotal - draft.Totals.GrandTotal) > 0.01m)
                return BadRequest("Totals mismatch.");

            if (draft.Payment == null)
                return BadRequest("Payment is required.");

            if (draft.Payment.AmountPaid < serverGrandTotal)
                return BadRequest("Underpayment not allowed.");

            var invoiceNumber = await GenerateInvoiceNumber(draft.ShopId);

            var sale = new Sale
            {
                InvoiceNumber = invoiceNumber,
                ShopId = draft.ShopId,
                UserId = draft.Cashier.UserId,
                SaleDate = now,
                SubTotal = serverSubTotal,
                TaxTotal = serverTax,
                SaleDiscount = serverDiscount,
                GrandTotal = serverGrandTotal,
                CurrencyCode = draft.CurrencyCode,
                CreatedAt = now,
                Status = STATUS_COMPLETED,
                Notes = draft.Payment.Notes,
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            foreach (var item in draft.Items)
            {
                var v = variants[item.VariantId];

                _context.SaleItems.Add(new SaleItem
                {
                    SaleId = sale.SaleId,
                    ShopId = draft.ShopId,
                    VariantId = v.VariantId,
                    Quantity = item.Qty,
                    ProductNameAtSale = item.Name,
                    VariantDescriptionAtSale = $"{item.Size} {item.Color}",
                    UnitPriceAtSale = v.SellingPrice,
                    UnitCostAtSale = v.CostPrice,
                    TotalCostAtSale = v.CostPrice * item.Qty,
                    TaxRateAtSale = TAX_PERCENT,
                    TaxAmountAtSale = v.SellingPrice * item.Qty * TAX_PERCENT / 100,
                    TotalPriceAtSale = v.SellingPrice * item.Qty,
                    CurrencyCode = draft.CurrencyCode,
                    CreatedAt = now
                });

                _context.StockMovements.Add(new StockMovement
                {
                    ShopId = draft.ShopId,
                    VariantId = v.VariantId,
                    MovementTypeId = SALE_MOVEMENT_TYPE,
                    QuantityChange = -item.Qty,
                    ReferenceId = sale.SaleId,
                    Reason = STOCK_REASON_SALE,
                    CreatedByUserId = draft.Cashier.UserId,
                    CreatedByUsername = draft.Cashier.Username,
                    CreatedAt = now
                });

                v.CurrentStock -= item.Qty;
            }

            _context.Payments.Add(new Payment
            {
                SaleId = sale.SaleId,
                Amount = draft.Payment.AmountPaid - draft.Payment.Change,
                Change = draft.Payment.Change,
                CurrencyCode = draft.CurrencyCode,
                PaymentMethod = draft.Payment.Method,
                PaymentDate = now,
                Status = STATUS_COMPLETED
            });

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return Ok(new
            {
                saleId = sale.SaleId,
                invoiceNumber = sale.InvoiceNumber
            });
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    private async Task<string> GenerateInvoiceNumber(int shopId)
    {
        var lastSale = await _context.Sales
            .Where(s => s.ShopId == shopId)
            .OrderByDescending(s => s.SaleId)
            .FirstOrDefaultAsync();

        var nextNumber = (lastSale?.SaleId ?? 0) + 1;
        return $"INV-{DateTime.UtcNow:yyyyMMdd}-{nextNumber:D6}";
    }
}
