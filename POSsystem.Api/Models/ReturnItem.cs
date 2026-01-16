using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POSsystem.Api.Models;

[Table("ReturnItem")]
public partial class ReturnItem
{
    [Key]
    [Column("ReturnItemID")]
    public int ReturnItemId { get; set; }

    [Column("SaleReturnID")]
    public int SaleReturnId { get; set; }

    [Column("SaleItemID")]
    public int SaleItemId { get; set; }

    public int QuantityReturned { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal UnitPriceAtReturn { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal TaxAmountAtReturn { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("SaleItemId")]
    [InverseProperty("ReturnItems")]
    public virtual SaleItem SaleItem { get; set; } = null!;

    [ForeignKey("SaleReturnId")]
    [InverseProperty("ReturnItems")]
    public virtual SaleReturn SaleReturn { get; set; } = null!;
}
