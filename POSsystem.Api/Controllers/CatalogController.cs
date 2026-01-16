using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSsystem.Api.Models;

namespace POSsystem.Api.Controllers;

[ApiController]
[Route("api/catalog")]
public class CatalogController : ControllerBase
{
    private readonly PosDbContext _context;

    public CatalogController(PosDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Returns all active products.
    /// </summary>
    /// <returns>List of products</returns>
    [HttpGet("products")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<object>>> GetProducts()
    {
        var products = await _context.Products
            .Where(p => p.IsActive)
            .Select(p => new
            {
                p.ProductId,
                p.Name,
                p.Brand,
                p.CategoryId
            })
            .ToListAsync();

        return Ok(products);
    }

    /// <summary>
    /// Returns all active variants for a specific product.
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <returns>List of variants</returns>
    [HttpGet("products/{productId:int}/variants")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<object>>> GetVariants(int productId)
    {
        var variants = await _context.ProductVariants
            .Where(v => v.ProductId == productId && v.IsActive)
            .Select(v => new
            {
                v.VariantId,
                v.Barcode,
                v.Sku,
                v.SellingPrice,
                v.CurrentStock,
                v.Notes
            })
            .ToListAsync();

        return Ok(variants);
    }

    /// <summary>
    /// Lookup a product variant by barcode (cashier workflow).
    /// </summary>
    /// <param name="barcode">Scanned barcode</param>
    /// <returns>Variant details</returns>
    [HttpGet("variant-by-barcode/{barcode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetVariantByBarcode(string barcode)
    {
        var variant = await _context.ProductVariants
            .Where(v => v.Barcode == barcode && v.IsActive)
            .Select(v => new
            {
                v.VariantId,
                v.Barcode,
                v.SellingPrice,
                v.CurrentStock,
                ProductName = v.Product.Name
            })
            .FirstOrDefaultAsync();

        if (variant == null)
            return NotFound("Barcode not found");

        return Ok(variant);
    }
}
