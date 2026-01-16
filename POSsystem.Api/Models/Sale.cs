using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POSsystem.Api.Models;

[Table("Sale")]
[Index("ShopId", "SaleDate", Name = "IX_Sale_Shop_Date", IsDescending = new[] { false, true })]
[Index("ShopId", "InvoiceNumber", Name = "UQ_Sale_Shop_InvoiceNumber", IsUnique = true)]
public partial class Sale
{
    [Key]
    [Column("SaleID")]
    public int SaleId { get; set; }

    [Column("ShopID")]
    public int ShopId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    public DateTime SaleDate { get; set; }

    [StringLength(50)]
    public string InvoiceNumber { get; set; } = null!;

    [Column(TypeName = "decimal(12, 2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal SaleDiscount { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal TaxTotal { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal GrandTotal { get; set; }

    [StringLength(3)]
    [Unicode(false)]
    public string CurrencyCode { get; set; } = null!;

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [InverseProperty("Sale")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("Sale")]
    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

    [InverseProperty("Sale")]
    public virtual ICollection<SaleReturn> SaleReturns { get; set; } = new List<SaleReturn>();

    [ForeignKey("ShopId")]
    [InverseProperty("Sales")]
    public virtual Shop Shop { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Sales")]
    public virtual User User { get; set; } = null!;
}
