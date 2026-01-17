using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POSsystem.Api.Models;

[Table("SaleItem")]
[Index("SaleId", Name = "IX_SaleItem_Sale")]
public partial class SaleItem
{
    [Key]
    [Column("SaleItemID")]
    public int SaleItemId { get; set; }

    [Column("SaleID")]
    public int SaleId { get; set; }

    [Column("ShopID")]
    public int ShopId { get; set; }

    [Column("VariantID")]
    public int VariantId { get; set; }

    public int Quantity { get; set; }

    [StringLength(200)]
    public string ProductNameAtSale { get; set; } = null!;

    [StringLength(200)]
    public string? VariantDescriptionAtSale { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal UnitPriceAtSale { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal UnitCostAtSale { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal TotalCostAtSale { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal TaxRateAtSale { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal TaxAmountAtSale { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal TotalPriceAtSale { get; set; }

    [StringLength(3)]
    [Unicode(false)]
    public string CurrencyCode { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [InverseProperty("SaleItem")]
    public virtual ICollection<ReturnItem> ReturnItems { get; set; } = new List<ReturnItem>();

    [ForeignKey("SaleId")]
    [InverseProperty("SaleItems")]
    public virtual Sale Sale { get; set; } = null!;

    [ForeignKey("ShopId")]
    [InverseProperty("SaleItems")]
    public virtual Shop Shop { get; set; } = null!;

    [ForeignKey("VariantId")]
    [InverseProperty("SaleItems")]
    public virtual ProductVariant Variant { get; set; } = null!;
}
