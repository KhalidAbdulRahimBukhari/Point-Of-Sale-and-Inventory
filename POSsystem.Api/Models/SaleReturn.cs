using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POSsystem.Api.Models;

[Table("SaleReturn")]
public partial class SaleReturn
{
    [Key]
    [Column("SaleReturnID")]
    public int SaleReturnId { get; set; }

    [Column("SaleID")]
    public int SaleId { get; set; }

    [Column("ShopID")]
    public int ShopId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    public DateTime ReturnDate { get; set; }

    [StringLength(300)]
    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalRefundAmount { get; set; }

    [StringLength(20)]
    public string ReturnType { get; set; } = null!;

    [InverseProperty("SaleReturn")]
    public virtual ICollection<ReturnItem> ReturnItems { get; set; } = new List<ReturnItem>();

    [ForeignKey("SaleId")]
    [InverseProperty("SaleReturns")]
    public virtual Sale Sale { get; set; } = null!;

    [ForeignKey("ShopId")]
    [InverseProperty("SaleReturns")]
    public virtual Shop Shop { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("SaleReturns")]
    public virtual User User { get; set; } = null!;
}
