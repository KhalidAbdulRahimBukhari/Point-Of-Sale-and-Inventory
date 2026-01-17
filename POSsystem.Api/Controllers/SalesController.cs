using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    // NOTE: Transaction + stock + payment handled here (later)
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult CreateSale()
    {
        // Placeholder – real implementation comes next
        return StatusCode(StatusCodes.Status501NotImplemented,
            "Sale creation logic not implemented yet");
    }
}
