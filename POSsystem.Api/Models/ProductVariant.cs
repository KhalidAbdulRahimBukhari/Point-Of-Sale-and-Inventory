using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POSsystem.Api.Models;

[Table("ProductVariant")]
[Index("ProductId", "Size", "Color", Name = "UQ_ProductVariant_Product_Size_Color", IsUnique = true)]
[Index("ShopId", "Barcode", Name = "UQ_ProductVariant_Shop_Barcode", IsUnique = true)]
public partial class ProductVariant
{
    [Key]
    [Column("VariantID")]
    public int VariantId { get; set; }

    [Column("ShopID")]
    public int ShopId { get; set; }

    [Column("ProductID")]
    public int ProductId { get; set; }

    [Column("SKU")]
    [StringLength(50)]
    public string? Sku { get; set; }

    [StringLength(50)]
    public string Barcode { get; set; } = null!;

    [StringLength(50)]
    public string? Size { get; set; }

    [StringLength(50)]
    public string? Color { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal SellingPrice { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal CostPrice { get; set; }

    public int CurrentStock { get; set; }

    public int TotalSoldQty { get; set; }

    public int? LowStockThreshold { get; set; }

    [StringLength(500)]
    public string? ImagePath { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductVariants")]
    public virtual Product Product { get; set; } = null!;

    [InverseProperty("Variant")]
    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

    [ForeignKey("ShopId")]
    [InverseProperty("ProductVariants")]
    public virtual Shop Shop { get; set; } = null!;

    [InverseProperty("Variant")]
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
