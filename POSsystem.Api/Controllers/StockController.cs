using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSsystem.Api.Models;

namespace POSsystem.Api.Controllers;

[ApiController]
[Route("api/stock")]
public class StockController : ControllerBase
{
    private readonly PosDbContext _context;

    public StockController(PosDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Returns current stock for all active product variants.
    /// Stock is derived from movements, not blindly trusted.
    /// </summary>
    [HttpGet("variants")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<object>>> GetAllVariantStock()
    {
        var stock = await _context.ProductVariants
            .Where(v => v.IsActive)
            .Select(v => new
            {
                v.VariantId,
                v.Barcode,
                ProductName = v.Product.Name,
                v.CurrentStock
            })
            .ToListAsync();

        return Ok(stock);
    }

    /// <summary>
    /// Returns stock and movement history for a single variant.
    /// </summary>
    /// <param name="variantId">Variant ID</param>
    [HttpGet("variants/{variantId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetVariantStock(int variantId)
    {
        var variant = await _context.ProductVariants
            .Where(v => v.VariantId == variantId)
            .Select(v => new
            {
                v.VariantId,
                v.Barcode,
                ProductName = v.Product.Name,
                v.CurrentStock
            })
            .FirstOrDefaultAsync();

        if (variant == null)
            return NotFound("Variant not found");

        return Ok(variant);
    }

    /// <summary>
    /// Returns stock movement ledger for a variant.
    /// This is the source of truth.
    /// </summary>
    /// <param name="variantId">Variant ID</param>
    [HttpGet("variants/{variantId:int}/movements")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<object>>> GetVariantMovements(int variantId)
    {
        var movements = await _context.StockMovements
            .Where(m => m.VariantId == variantId)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new
            {
                m.StockMovementId,
                m.MovementType,
                m.QuantityChange,
                m.ReferenceId,
                m.CreatedAt
            })
            .ToListAsync();

        return Ok(movements);
    }

    /// <summary>
    /// Verifies stock consistency by comparing
    /// SUM(movements) vs stored CurrentStock.
    /// </summary>
    [HttpGet("audit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<object>>> AuditStock()
    {
        var audit = await _context.ProductVariants
            .Select(v => new
            {
                v.VariantId,
                v.Barcode,
                StoredStock = v.CurrentStock,
                CalculatedStock = _context.StockMovements
                    .Where(m => m.VariantId == v.VariantId)
                    .Sum(m => m.QuantityChange)
            })
            .ToListAsync();

        return Ok(audit);
    }
}
