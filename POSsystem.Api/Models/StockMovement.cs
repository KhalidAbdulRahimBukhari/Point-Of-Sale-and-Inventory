using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POSsystem.Api.Models;

[Table("StockMovement")]
[Index("VariantId", "CreatedAt", Name = "IX_StockMovement_Variant_Date", IsDescending = new[] { false, true })]
public partial class StockMovement
{
    [Key]
    [Column("StockMovementID")]
    public long StockMovementId { get; set; }

    [Column("ShopID")]
    public int ShopId { get; set; }

    [Column("VariantID")]
    public int VariantId { get; set; }

    [Column("MovementTypeID")]
    public int MovementTypeId { get; set; }

    public int QuantityChange { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal UnitCost { get; set; }

    [Column("ReferenceID")]
    public int? ReferenceId { get; set; }

    [StringLength(300)]
    public string? Reason { get; set; }

    [Column("CreatedByUserID")]
    public int CreatedByUserId { get; set; }

    [StringLength(100)]
    public string CreatedByUsername { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("StockMovements")]
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("MovementTypeId")]
    [InverseProperty("StockMovements")]
    public virtual MovementType MovementType { get; set; } = null!;

    [ForeignKey("ShopId")]
    [InverseProperty("StockMovements")]
    public virtual Shop Shop { get; set; } = null!;

    [ForeignKey("VariantId")]
    [InverseProperty("StockMovements")]
    public virtual ProductVariant Variant { get; set; } = null!;
}
