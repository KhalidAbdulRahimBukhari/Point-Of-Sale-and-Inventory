using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSsystem.Api.DTOs;
using POSsystem.Api.Models;

[ApiController]
[Route("api/products")]
[Authorize(Roles = "Admin,Cashier")]
public class ProductsController : ControllerBase
{
    private const int SHOP_ID = 1;
    private readonly PosDbContext _context;

    public ProductsController(PosDbContext context)
    {
        _context = context;
    }

    // -------------------- GET ALL --------------------

    [HttpGet]
    [ProducesResponseType(typeof(List<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<ProductResponse>>> GetAll()
    {
        var data = await _context.ProductVariants
            .AsNoTracking()
            .Where(v => v.ShopId == SHOP_ID)
            .Select(v => new ProductResponse
            {
                ProductId = v.ProductId,
                VariantId = v.VariantId,
                Name = v.Product.Name,
                Description = v.Product.Description,
                Brand = v.Product.Brand,
                ImagePath = v.ImagePath ?? v.Product.ImagePath,
                Category = v.Product.Category.Name,
                CategoryId = v.Product.CategoryId,
                Barcode = v.Barcode,
                SKU = v.Sku,
                Size = v.Size,
                Color = v.Color,
                SellingPrice = v.SellingPrice,
                CostPrice = v.CostPrice,
                CurrentStock = v.CurrentStock
            })
            .OrderBy(p => p.Name)
            .ToListAsync();

        return Ok(data);
    }

    // -------------------- GET BY VARIANT --------------------

    [HttpGet("{variantId:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProductResponse>> GetByVariantId(int variantId)
    {
        var v = await _context.ProductVariants
            .Include(x => x.Product)
            .ThenInclude(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.VariantId == variantId && x.ShopId == SHOP_ID);

        if (v == null)
            return NotFound();

        return Ok(MapToResponse(v));
    }

    // -------------------- CREATE --------------------

    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponse>> Create(ProductUpsertRequest req)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == req.CategoryId && c.ShopId == SHOP_ID);

        if (category == null)
            return BadRequest("Invalid category");

        var product = new Product
        {
            ShopId = SHOP_ID,
            Name = req.Name,
            Description = req.Description,
            Brand = req.Brand,
            ImagePath = req.ImagePath,
            CategoryId = req.CategoryId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var variant = new ProductVariant
        {
            ShopId = SHOP_ID,
            ProductId = product.ProductId,
            Barcode = req.Barcode,
            Sku = req.SKU,
            Size = req.Size,
            Color = req.Color,
            SellingPrice = req.SellingPrice,
            CostPrice = req.CostPrice,
            CurrentStock = req.CurrentStock,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.ProductVariants.Add(variant);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetByVariantId),
            new { variantId = variant.VariantId },
            MapToResponse(variant, product, category.Name)
        );
    }

    // -------------------- UPDATE --------------------

    [HttpPut("{variantId:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> Update(int variantId, ProductUpsertRequest req)
    {
        var v = await _context.ProductVariants
            .Include(x => x.Product)
            .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(x => x.VariantId == variantId && x.ShopId == SHOP_ID);

        if (v == null)
            return NotFound();

        var categoryExists = await _context.Categories
            .AnyAsync(c => c.CategoryId == req.CategoryId && c.ShopId == SHOP_ID);

        if (!categoryExists)
            return BadRequest("Invalid category");

        // Product
        v.Product.Name = req.Name;
        v.Product.Description = req.Description;
        v.Product.Brand = req.Brand;
        v.Product.ImagePath = req.ImagePath;
        v.Product.CategoryId = req.CategoryId;

        // Variant
        v.Barcode = req.Barcode;
        v.Sku = req.SKU;
        v.Size = req.Size;
        v.Color = req.Color;
        v.SellingPrice = req.SellingPrice;
        v.CostPrice = req.CostPrice;
        v.CurrentStock = req.CurrentStock;

        await _context.SaveChangesAsync();

        return Ok(MapToResponse(v));
    }

    // -------------------- STOCK IN --------------------

    [HttpPost("{variantId:int}/stock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StockIn(int variantId, [FromQuery] int quantity)
    {
        if (quantity <= 0)
            return BadRequest("Quantity must be greater than zero");

        var v = await _context.ProductVariants
            .FirstOrDefaultAsync(x => x.VariantId == variantId && x.ShopId == SHOP_ID);

        if (v == null)
            return NotFound();

        v.CurrentStock += quantity;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // -------------------- GET CATEGORIES --------------------

    [HttpGet("categories")]
    [ProducesResponseType(typeof(List<CategoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoryResponse>>> GetCategories()
    {
        var categories = await _context.Categories
            .Where(c => c.ShopId == SHOP_ID)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponse
            {
                CategoryId = c.CategoryId,
                Name = c.Name
            })
            .ToListAsync();

        return Ok(categories);
    }

    // -------------------- MAPPING --------------------

    private static ProductResponse MapToResponse(
        ProductVariant v,
        Product? product = null,
        string? categoryName = null)
    {
        var p = product ?? v.Product;

        return new ProductResponse
        {
            ProductId = v.ProductId,
            VariantId = v.VariantId,
            Name = p.Name,
            Description = p.Description,
            Brand = p.Brand,
            ImagePath = v.ImagePath ?? p.ImagePath,
            Category = categoryName ?? p.Category?.Name ?? string.Empty,
            CategoryId = p.CategoryId,
            Barcode = v.Barcode,
            SKU = v.Sku,
            Size = v.Size,
            Color = v.Color,
            SellingPrice = v.SellingPrice,
            CostPrice = v.CostPrice,
            CurrentStock = v.CurrentStock
        };
    }
}
