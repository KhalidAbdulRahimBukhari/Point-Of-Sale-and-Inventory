using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSsystem.Api.DTOs;
using POSsystem.Api.Models;

namespace POSsystem.Api.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    [Authorize(Roles = "Admin,Cashier")]
    public class InvoiceController : ControllerBase
    {
        private readonly PosDbContext _context;

        public InvoiceController(PosDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all invoices with their items.
        /// </summary>
        /// <response code="200">Invoices retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<InvoiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<InvoiceDto>>> GetAllInvoices()
        {
            try
            {
                var headers = await _context.Set<InvoiceHeaderView>()
                    .AsNoTracking()
                    .ToListAsync();

                if (!headers.Any())
                    return Ok(new List<InvoiceDto>());

                var saleIds = headers.Select(h => h.SaleID).ToList();

                var items = await _context.Set<InvoiceItemView>()
                    .AsNoTracking()
                    .Where(i => saleIds.Contains(i.SaleID))
                    .ToListAsync();

                var itemsGrouped = items
                    .GroupBy(i => i.SaleID)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var invoices = headers.Select(h => new InvoiceDto
                {
                    InvoiceNo = h.InvoiceNumber,
                    CreatedAt = h.CreatedAt,
                    Cashier = h.CashierUserName,

                    Subtotal = h.SubTotal,
                    Tax = h.TaxTotal,
                    Discount = h.SaleDiscount,
                    Total = h.GrandTotal,

                    AmountPaid = h.AmountPaid,
                    Change = h.ChangeAmount,
                    PaymentMethod = h.PaymentMethod,

                    Items = itemsGrouped.ContainsKey(h.SaleID)
                        ? itemsGrouped[h.SaleID]
                            .Select(i => new InvoiceItemDto
                            {
                                Name = i.Name,
                                Size = i.Size,
                                Price = i.Price,
                                Qty = i.Qty
                            }).ToList()
                        : new List<InvoiceItemDto>()
                }).ToList();

                return Ok(invoices);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.");
            }
        }
    }
}
