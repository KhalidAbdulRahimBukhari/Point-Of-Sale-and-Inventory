using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSsystem.Api.DTOs;
using POSsystem.Api.Models;

[ApiController]
[Route("api/sales/returns")]
[Authorize(Roles = "Admin,Cashier")]
public class SaleReturnController : ControllerBase
{
    private readonly PosDbContext _context;

    public SaleReturnController(PosDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateReturn([FromBody] CreateSaleReturnDto dto)
    {
        var sale = await _context.Sales
     .Include(s => s.SaleItems)
         .ThenInclude(si => si.ReturnItems)
     .FirstOrDefaultAsync(s =>
         s.InvoiceNumber == dto.InvoiceNumber &&
         s.Status == "COMPLETED");


        if (sale == null)
            return BadRequest("Sale not found or not completed");

        if (dto.Items == null || !dto.Items.Any())
            return BadRequest("No return items provided");

        // 2️⃣ Pre-validate return quantities
        foreach (var item in dto.Items)
        {
            var saleItem = sale.SaleItems
                .FirstOrDefault(si => si.SaleItemId == item.SaleItemId);

            if (saleItem == null)
                return BadRequest($"Invalid SaleItemId {item.SaleItemId}");

            var alreadyReturned = saleItem.ReturnItems?.Sum(r => r.QuantityReturned) ?? 0;
            var remainingQty = saleItem.Quantity - alreadyReturned;

            if (item.QuantityToReturn <= 0 || item.QuantityToReturn > remainingQty)
                return BadRequest($"Invalid return quantity for SaleItem {saleItem.SaleItemId}");
        }

        // 3️⃣ Start transaction ONLY when ready to write
        using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            var saleReturn = new SaleReturn
            {
                SaleId = sale.SaleId,
                ShopId = sale.ShopId,
                ReturnDate = DateTime.UtcNow,
                UserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value),
                TotalRefundAmount = 0
            };

            _context.SaleReturns.Add(saleReturn);
            await _context.SaveChangesAsync();

            decimal totalRefund = 0;

            foreach (var item in dto.Items)
            {
                var saleItem = sale.SaleItems
                    .First(si => si.SaleItemId == item.SaleItemId);

                var unitTax =
                    saleItem.Quantity > 0
                        ? saleItem.TaxAmountAtSale / saleItem.Quantity
                        : 0;

                var refundAmount = saleItem.UnitPriceAtSale * item.QuantityToReturn;
                var refundTax = unitTax * item.QuantityToReturn;

                _context.ReturnItems.Add(new ReturnItem
                {
                    SaleReturnId = saleReturn.SaleReturnId,
                    SaleItemId = saleItem.SaleItemId,
                    QuantityReturned = item.QuantityToReturn,
                    RefundAmount = refundAmount,
                    TaxRefundAmount = refundTax
                });

                // 4️⃣ Lock & update stock INSIDE transaction
                var variant = await _context.ProductVariants
                    .Where(v => v.VariantId == saleItem.VariantId)
                    .FirstAsync();

                variant.CurrentStock += item.QuantityToReturn;

                _context.StockMovements.Add(new StockMovement
                {
                    VariantId = variant.VariantId,
                    ShopId = variant.ShopId,
                    QuantityChange = item.QuantityToReturn,
                    MovementTypeId = 2, // RETURN
                    ReferenceId = saleReturn.SaleReturnId,
                    CreatedByUserId = saleReturn.UserId,
                    CreatedAt = DateTime.UtcNow
                });

                totalRefund += refundAmount + refundTax;
            }

            saleReturn.TotalRefundAmount = totalRefund;
            saleReturn.ReturnType =
                totalRefund >= sale.GrandTotal ? "FULL" : "PARTIAL";

            _context.Payments.Add(new Payment
            {
                SaleId = sale.SaleId,
                Amount = -totalRefund,
                PaymentMethod = dto.PaymentMethod,
                Status = "REFUNDED",
                PaymentDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return CreatedAtAction(nameof(CreateReturn), new
            {
                saleReturn.SaleReturnId,
                sale.InvoiceNumber,
                saleReturn.TotalRefundAmount,
                saleReturn.ReturnType
            });
        }
        catch
        {
            await tx.RollbackAsync();
            throw; // real failure, not business logic
        }
    }
}
