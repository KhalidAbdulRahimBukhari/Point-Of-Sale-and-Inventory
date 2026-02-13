using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POSsystem.Api.Models;

[Table("Payment")]
[Index("SaleId", Name = "IX_Payment_Sale")]
public partial class Payment
{
    [Key]
    [Column("PaymentID")]
    public int PaymentId { get; set; }

    [Column("SaleID")]
    public int SaleId { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal Change { get; set; }

    [StringLength(3)]
    [Unicode(false)]
    public string CurrencyCode { get; set; } = null!;

    [StringLength(50)]
    public string PaymentMethod { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [ForeignKey("SaleId")]
    [InverseProperty("Payments")]
    public virtual Sale Sale { get; set; } = null!;
}
