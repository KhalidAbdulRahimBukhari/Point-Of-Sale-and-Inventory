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

    public SaleController(PosDbContext context)
    {
        _context = context;
    }

    // 1️⃣ Get sales (optional date filter)
    // GET: api/sales?from=2024-01-01&to=2024-01-31
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<object>>> GetSales(
        DateTime? from,
        DateTime? to)
    {
        var query = _context.Sales.AsQueryable();

        if (from.HasValue)
            query = query.Where(s => s.SaleDate >= from.Value);

        if (to.HasValue)
            query = query.Where(s => s.SaleDate <= to.Value);

        var sales = await query
            .OrderByDescending(s => s.SaleDate)
            .Select(s => new
            {
                s.SaleId,
                s.InvoiceNumber,
                s.SaleDate,
                s.GrandTotal,
                s.Status,
                s.UserId
            })
            .ToListAsync();

        return Ok(sales);
    }

    // 2️⃣ Find sale by invoice number
    // GET: api/sales/invoice/INV-1001
    [HttpGet("invoice/{invoiceNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetSaleByInvoice(string invoiceNumber)
    {
        var sale = await _context.Sales
            .Where(s => s.InvoiceNumber == invoiceNumber)
            .Select(s => new
            {
                s.SaleId,
                s.InvoiceNumber,
                s.SaleItems,
                s.SaleDate,
                s.SubTotal,
                s.SaleDiscount,
                s.TaxTotal,
                s.GrandTotal,
                s.Status
            })
            .FirstOrDefaultAsync();

        if (sale == null)
            return NotFound("Sale not found");

        return Ok(sale);
    }

    // 3️⃣ Get full sale details (items + payments)
    // GET: api/sales/{id}
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetSaleDetails(int id)
    {
        var sale = await _context.Sales
            .Where(s => s.SaleId == id)
            .Select(s => new
            {
                s.SaleId,
                s.InvoiceNumber,
                s.SaleDate,
                s.GrandTotal,
                Items = s.SaleItems.Select(i => new
                {
                    i.VariantId,
                    i.Quantity,
                    i.UnitPriceAtSale,
                    i.TotalPriceAtSale
                }),
                Payments = s.Payments.Select(p => new
                {
                    p.Amount,
                    p.PaymentMethod,
                    p.Status,
                    p.PaymentDate
                })
            })
            .FirstOrDefaultAsync();

        if (sale == null)
            return NotFound("Sale not found");

        return Ok(sale);
    }

    // 4️⃣ Create a new sale
    // POST: api/sales
    // NOTE: Transaction + stock + payment handled here
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
            return BadRequest("Sale must contain at least one item");

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1️⃣ Generate invoice number (MVP-safe, not concurrency-safe)
            var today = DateTime.UtcNow.Date;
            var countToday = await _context.Sales.CountAsync(s => s.SaleDate.Date == today);
            var invoiceNumber = $"INV-{today:yyyyMMdd}-{(countToday + 1):D6}";

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var username = User.Identity!.Name ?? "system";

            // 2️⃣ Create Sale header
            var sale = new Sale
            {
                InvoiceNumber = invoiceNumber,
                SaleDate = DateTime.UtcNow,
                Status = "COMPLETED",
                UserId = userId,
                SubTotal = 0,
                TaxTotal = 0,
                SaleDiscount = dto.SaleDiscount,
                GrandTotal = 0,
                Notes = dto.Notes
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync(); // get SaleId

            decimal subTotal = 0;
            decimal taxTotal = 0;
            const decimal TAX_RATE = 0.15m;

            // 3️⃣ Process sale items
            foreach (var item in dto.Items)
            {
                if (item.Quantity <= 0)
                    throw new Exception("Invalid quantity");

                if (string.IsNullOrWhiteSpace(item.Barcode) && string.IsNullOrWhiteSpace(item.SKU))
                    throw new Exception("Barcode or SKU is required");

                var variant = await _context.ProductVariants
                    .Include(v => v.Product)
                    .Include(v => v.Shop)
                    .FirstOrDefaultAsync(v =>
                        (!string.IsNullOrEmpty(item.Barcode) && v.Barcode == item.Barcode) ||
                        (!string.IsNullOrEmpty(item.SKU) && v.Sku == item.SKU));

                if (variant == null)
                    throw new Exception("Product variant not found");

                if (variant.CurrentStock < item.Quantity)
                    throw new Exception("Insufficient stock");

                if (variant.SellingPrice <= 0)
                    throw new Exception("Invalid selling price");

                var lineTotal = variant.SellingPrice * item.Quantity;
                var lineTax = lineTotal * TAX_RATE;

                subTotal += lineTotal;
                taxTotal += lineTax;

                // SaleItem snapshot
                var saleItem = new SaleItem
                {
                    SaleId = sale.SaleId,
                    VariantId = variant.VariantId,
                    ShopId = variant.ShopId,

                    ProductNameAtSale = variant.Product.Name,
                    VariantDescriptionAtSale = variant.Notes,

                    Quantity = item.Quantity,
                    UnitPriceAtSale = variant.SellingPrice,
                    TotalPriceAtSale = lineTotal,

                    UnitCostAtSale = variant.CostPrice,
                    TotalCostAtSale = variant.CostPrice * item.Quantity,

                    TaxRateAtSale = TAX_RATE,
                    TaxAmountAtSale = lineTax,

                    CurrencyCode = variant.Shop.CurrencyCode,
                    CreatedAt = DateTime.UtcNow
                };

                _context.SaleItems.Add(saleItem);

                // Stock movement (SALE)
                var stockMovement = new StockMovement
                {
                    VariantId = variant.VariantId,
                    ShopId = variant.ShopId,
                    QuantityChange = -item.Quantity,
                    MovementTypeId = 1, // SALE
                    ReferenceId = sale.SaleId,
                    CreatedByUserId = userId,
                    CreatedByUsername = username,
                    CreatedAt = DateTime.UtcNow
                };

                _context.StockMovements.Add(stockMovement);

                // Update stock snapshot
                variant.CurrentStock -= item.Quantity;
            }

            // 4️⃣ Validate discount
            if (dto.SaleDiscount < 0)
                throw new Exception("Invalid discount");

            if (dto.SaleDiscount > subTotal)
                throw new Exception("Discount exceeds subtotal");

            // 5️⃣ Final totals
            sale.SubTotal = subTotal;
            sale.TaxTotal = taxTotal;
            sale.GrandTotal = subTotal + taxTotal - dto.SaleDiscount;

            _context.Sales.Update(sale);

            // 6️⃣ Payment (no partial payments in MVP)
            if (dto.PaidAmount != sale.GrandTotal)
                throw new Exception("Full payment is required");

            var payment = new Payment
            {
                SaleId = sale.SaleId,
                Amount = dto.PaidAmount,
                PaymentMethod = dto.PaymentMethod,
                Status = "PAID",
                PaymentDate = DateTime.UtcNow
            };

            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return CreatedAtAction(nameof(GetSaleDetails), new { id = sale.SaleId }, new
            {
                sale.SaleId,
                sale.InvoiceNumber,
                sale.SubTotal,
                sale.TaxTotal,
                sale.SaleDiscount,
                sale.GrandTotal,
                PaymentStatus = payment.Status
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return BadRequest(ex.Message);
        }
    }

}