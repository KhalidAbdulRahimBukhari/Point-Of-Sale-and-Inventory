using System.ComponentModel.DataAnnotations;

namespace POSsystem.Api.DTOs
{
    // Request DTO for creating/updating product with variant
    public class ProductUpsertRequest
    {
        // Product
        [Required, StringLength(200)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
        public string? Brand { get; set; }
        public string? ImagePath { get; set; }

        [Required]
        public int CategoryId { get; set; }

        // Variant
        [Required, StringLength(50)]
        public string Barcode { get; set; } = null!;

        public string? SKU { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }

        [Required, Range(0.01, double.MaxValue)]
        public decimal SellingPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal CostPrice { get; set; }

        // Inventory — ABSOLUTE value
        [Range(0, int.MaxValue)]
        public int CurrentStock { get; set; }
    }


    public class ProductResponse
    {
        public int ProductId { get; set; }
        public int VariantId { get; set; }

        public string Name { get; set; } = null!;
        public string Category { get; set; } = null!;
        public int CategoryId { get; set; }

        public string? Description { get; set; }
        public string? Brand { get; set; }
        public string? ImagePath { get; set; }

        public string Barcode { get; set; } = null!;
        public string? SKU { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }

        public decimal SellingPrice { get; set; }
        public decimal CostPrice { get; set; }
        public int CurrentStock { get; set; }
    }

    public class CategoryResponse
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
    }



}
