using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSsystem.Api.DTOs;
using POSsystem.Api.Models;

[ApiController]
[Route("api/sales")]
public class SaleController : ControllerBase
{
    private readonly PosDbContext _context;

    public SaleController(PosDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateSale([FromBody] SaleDraftDto draft)
    {


        using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            const decimal TAX_PERCENT = 15m;
            const int SALE_MOVEMENT_TYPE = 1;

            // 1. Load variants
            var variantIds = draft.Items.Select(i => i.VariantId).ToList();

            var variants = await _context.ProductVariants
                .Where(v => variantIds.Contains(v.VariantId))
                .ToDictionaryAsync(v => v.VariantId);

            if (variants.Count != variantIds.Count)
                return BadRequest("Invalid product variant.");

            // 2. Validate stock
            foreach (var item in draft.Items)
            {
                if (variants[item.VariantId].CurrentStock < item.Qty)
                    return BadRequest($"Insufficient stock for {item.Name}");
            }

            // 3. Recalculate totals (server truth)
            var serverSubTotal = draft.Items.Sum(i =>
                variants[i.VariantId].SellingPrice * i.Qty);

            var serverTax = serverSubTotal * TAX_PERCENT / 100;
            var serverDiscount =
                serverSubTotal * Math.Min(draft.Totals.DiscountPercentage, 15) / 100;

            var serverGrandTotal = serverSubTotal + serverTax - serverDiscount;

            // 4. Validate totals (anti-tamper)
            if (Math.Abs(serverGrandTotal - draft.Totals.GrandTotal) > 0.01m)
                return BadRequest("Totals mismatch.");

            // 5. Validate payment
            if (draft.Payment == null)
                return BadRequest("Payment is required.");

            if (draft.Payment.AmountPaid < serverGrandTotal)
                return BadRequest("Underpayment not allowed.");

            // Generate invoice number
            var invoiceNumber = await GenerateInvoiceNumber(draft.ShopId);

            // 6. Create Sale
            var sale = new Sale
            {
                InvoiceNumber = invoiceNumber,
                ShopId = draft.ShopId,
                UserId = draft.Cashier.UserId,
                SaleDate = DateTime.UtcNow,
                SubTotal = serverSubTotal,
                TaxTotal = serverTax,
                SaleDiscount = serverDiscount,
                GrandTotal = serverGrandTotal,
                CurrencyCode = draft.CurrencyCode,
                CreatedAt = DateTime.UtcNow,
                Status = "Completed",
                Notes = draft.Payment.Notes,
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            // 7. SaleItems + StockMovements + Stock update
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
                    CreatedAt = DateTime.UtcNow
                });

                _context.StockMovements.Add(new StockMovement
                {
                    ShopId = draft.ShopId,
                    VariantId = v.VariantId,
                    MovementTypeId = SALE_MOVEMENT_TYPE,
                    QuantityChange = -item.Qty,
                    ReferenceId = sale.SaleId,
                    Reason = "Sale",
                    CreatedByUserId = draft.Cashier.UserId,
                    CreatedByUsername = draft.Cashier.Username,
                    CreatedAt = DateTime.UtcNow
                });

                v.CurrentStock -= item.Qty;
            }

            // 8. Payment
            _context.Payments.Add(new Payment
            {
                SaleId = sale.SaleId,
                Amount = draft.Payment.AmountPaid - draft.Payment.Change,
                Change = draft.Payment.Change,
                CurrencyCode = draft.CurrencyCode,
                PaymentMethod = draft.Payment.Method,
                PaymentDate = DateTime.UtcNow,
                Status = "Completed"
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

    // Helper method
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
