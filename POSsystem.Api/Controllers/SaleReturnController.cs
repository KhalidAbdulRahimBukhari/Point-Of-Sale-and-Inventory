using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSsystem.Api.Models;
using POSsystem.Api.DTOs;

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

    // POST: api/sales/returns
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateReturn([FromBody] CreateSaleReturnDto dto)
    {
        using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1️⃣ Load sale by invoice with items + previous returns
            var sale = await _context.Sales
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.ReturnItems)
                .FirstOrDefaultAsync(s =>
                    s.InvoiceNumber == dto.InvoiceNumber &&
                    s.Status == "COMPLETED");

            if (sale == null)
                return BadRequest("Sale not found or not completed");

            // 2️⃣ Create SaleReturn header
            var saleReturn = new SaleReturn
            {
                SaleId = sale.SaleId,
                ReturnDate = DateTime.UtcNow,
                UserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value),
                TotalRefundAmount = 0
            };

            _context.SaleReturns.Add(saleReturn);
            await _context.SaveChangesAsync();

            decimal totalRefund = 0;

            // 3️⃣ Process return items
            foreach (var item in dto.Items)
            {
                var saleItem = sale.SaleItems
                    .FirstOrDefault(si => si.SaleItemId == item.SaleItemId);

                if (saleItem == null)
                    throw new Exception("Invalid SaleItem");

                var alreadyReturned = saleItem.ReturnItems.Sum(r => r.QuantityReturned);
                var remainingQty = saleItem.Quantity - alreadyReturned;

                if (item.QuantityToReturn <= 0 || item.QuantityToReturn > remainingQty)
                    throw new Exception("Invalid return quantity");

                var refundAmount = saleItem.UnitPriceAtSale * item.QuantityToReturn;
                var refundTax = saleItem.TaxAmountAtSale / saleItem.Quantity * item.QuantityToReturn;

                // ReturnItem
                var returnItem = new ReturnItem
                {
                    SaleReturnId = saleReturn.SaleReturnId,
                    SaleItemId = saleItem.SaleItemId,
                    QuantityReturned = item.QuantityToReturn,
                    UnitPriceAtReturn = saleItem.UnitPriceAtSale,
                    RefundAmount = refundAmount, 
                    TaxAmountAtReturn = refundTax
                };

                _context.ReturnItems.Add(returnItem);

                // StockMovement (RETURN)
                _context.StockMovements.Add(new StockMovement
                {
                    VariantId = saleItem.VariantId,
                    ShopId = saleItem.ShopId,
                    QuantityChange = item.QuantityToReturn,
                    MovementTypeId = 2, // RETURN
                    ReferenceId = saleReturn.SaleReturnId,
                    CreatedByUserId = saleReturn.UserId,
                    CreatedAt = DateTime.UtcNow
                });

                // Update stock snapshot
                var variant = await _context.ProductVariants.FindAsync(saleItem.VariantId);
                variant!.CurrentStock += item.QuantityToReturn;

                totalRefund += refundAmount + refundTax;
            }

            // 4️⃣ Finalize return totals
            saleReturn.TotalRefundAmount = totalRefund;
            saleReturn.ReturnType = totalRefund == sale.GrandTotal ? "FULL" : "PARTIAL";

            // 5️⃣ Refund payment (negative)
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
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return BadRequest(ex.Message);
        }
    }
}
