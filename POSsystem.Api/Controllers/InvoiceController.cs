using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSsystem.Api.DTOs;
using POSsystem.Api.Models;



namespace POSsystem.Api.Controllers
{

    [ApiController]
    [Route("api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly PosDbContext _context;

        public InvoiceController(PosDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<InvoiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<InvoiceDto>>> GetAllInvoices()
        {
            var headers = await _context.Set<InvoiceHeaderView>()
                .AsNoTracking()
                .ToListAsync();

            var saleIds = headers.Select(h => h.SaleID).ToList();

            var items = await _context.Set<InvoiceItemView>()
                .AsNoTracking()
                .Where(i => saleIds.Contains(i.SaleID))
                .ToListAsync();

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

                Items = items
                    .Where(i => i.SaleID == h.SaleID)
                    .Select(i => new InvoiceItemDto
                    {
                        Name = i.Name,
                        Size = i.Size,
                        Price = i.Price,
                        Qty = i.Qty
                    })
                    .ToList()
            }).ToList();

            return Ok(invoices);
        }


    }
}
