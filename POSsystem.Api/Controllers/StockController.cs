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


    [HttpGet]
    public async Task<ActionResult<List<ProductResponse>>> GetAll()
    {
        var data = await _context.ProductVariants
            .Include(v => v.Product)
            .ThenInclude(p => p.Category)
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

    [HttpGet("{variantId:int}")]
    public async Task<ActionResult<ProductResponse>> GetByVariantId(int variantId)
    {
        var v = await _context.ProductVariants
            .Include(x => x.Product)
            .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(x => x.VariantId == variantId && x.ShopId == SHOP_ID);

        if (v == null)
            return NotFound();

        return Ok(new ProductResponse
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
        });
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(ProductUpsertRequest req)
    {
        if (!await _context.Categories.AnyAsync(c => c.CategoryId == req.CategoryId && c.ShopId == SHOP_ID))
            return BadRequest("Invalid category");

        using var tx = await _context.Database.BeginTransactionAsync();

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
        await tx.CommitAsync();

        var categoryName = await _context.Categories
            .Where(c => c.CategoryId == req.CategoryId)
            .Select(c => c.Name)
            .FirstAsync();

        return Ok(new ProductResponse
        {
            ProductId = product.ProductId,
            VariantId = variant.VariantId,
            Name = product.Name,
            Description = product.Description,
            Brand = product.Brand,
            ImagePath = product.ImagePath,
            Category = categoryName,
            CategoryId = req.CategoryId,
            Barcode = variant.Barcode,
            SKU = variant.Sku,
            Size = variant.Size,
            Color = variant.Color,
            SellingPrice = variant.SellingPrice,
            CostPrice = variant.CostPrice,
            CurrentStock = variant.CurrentStock
        });
    }

    [HttpPut("{variantId:int}")]
    public async Task<ActionResult<ProductResponse>> Update(
    int variantId,
    ProductUpsertRequest req)
    {
        var v = await _context.ProductVariants
            .Include(x => x.Product)
            .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(x =>
                x.VariantId == variantId &&
                x.ShopId == SHOP_ID);

        if (v == null)
            return NotFound();

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

        // Inventory (absolute overwrite — intentional)
        v.CurrentStock = req.CurrentStock;

        await _context.SaveChangesAsync();

        return Ok(new ProductResponse
        {
            ProductId = v.ProductId,
            VariantId = v.VariantId,
            Name = v.Product.Name,
            Description = v.Product.Description,
            Brand = v.Product.Brand,
            ImagePath = v.Product.ImagePath,
            Category = v.Product.Category.Name,
            CategoryId = v.Product.CategoryId,
            Barcode = v.Barcode,
            SKU = v.Sku,
            Size = v.Size,
            Color = v.Color,
            SellingPrice = v.SellingPrice,
            CostPrice = v.CostPrice,
            CurrentStock = v.CurrentStock
        });
    }

    [HttpPost("{variantId:int}/stock")]
    public async Task<IActionResult> StockIn(
        int variantId,
        [FromQuery] int quantity)
    {
        var v = await _context.ProductVariants
            .FirstOrDefaultAsync(x => x.VariantId == variantId && x.ShopId == SHOP_ID);

        if (v == null)
            return NotFound();

        v.CurrentStock += quantity;


        await _context.SaveChangesAsync();
        return NoContent();
    }


    [HttpGet("categories")]
    public async Task<ActionResult<List<CategoryResponse>>> GetCategories()
    {
        var categories = await _context.Categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponse
            {
                CategoryId = c.CategoryId,
                Name = c.Name
            })
            .ToListAsync();

        return Ok(categories);
    }



}