using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSsystem.Api.Models;

namespace POSsystem.Api.Controllers
{
    /// <summary>
    /// Read-only stock endpoints.
    /// Requires authentication (JWT).
    /// </summary>
    [ApiController]
    [Route("api/stock")]
    [Authorize(Roles = "Admin,Cashier")] // 🔒 JWT REQUIRED for all endpoints in this controller
    public class StockController : ControllerBase
    {
        private readonly PosDbContext _context;

        public StockController(PosDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get current stock for all product variants in a shop.
        /// Used for inventory overview.
        /// </summary>
        /// <param name="shopId">Shop identifier</param>
        [HttpGet("current/{shopId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<object>>> GetCurrentStock(int shopId)
        {
            var stock = await _context.ProductVariants
                .Where(v => v.ShopId == shopId && v.IsActive)
                .Select(v => new
                {
                    v.VariantId,
                    ProductName = v.Product.Name,
                    v.Sku,
                    v.Barcode,
                    v.CurrentStock
                })
                .ToListAsync();

            if (!stock.Any())
                return NotFound("No stock found for this shop");

            return Ok(stock);
        }

        /// <summary>
        /// Get stock movement history for a specific variant.
        /// Used for auditing and reconciliation.
        /// </summary>
        /// <param name="variantId">Product variant ID</param>
        [HttpGet("movements/{variantId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<object>>> GetStockMovements(int variantId)
        {
            var movements = await _context.StockMovements
                .Where(m => m.VariantId == variantId)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new
                {
                    m.StockMovementId,
                    m.MovementType,
                    m.QuantityChange,
                    m.ReferenceId,
                    m.CreatedAt
                })
                .ToListAsync();

            if (!movements.Any())
                return NotFound("No stock movements found");

            return Ok(movements);
        }
    }
}
