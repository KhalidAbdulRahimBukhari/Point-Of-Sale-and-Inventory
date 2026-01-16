using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POSsystem.Api.Models;

[Table("Expense")]
[Index("ShopId", "ExpenseDate", Name = "IX_Expense_Shop_Date", IsDescending = new[] { false, true })]
public partial class Expense
{
    [Key]
    [Column("ExpenseID")]
    public int ExpenseId { get; set; }

    [Column("ShopID")]
    public int ShopId { get; set; }

    [Column("ExpenseCategoryID")]
    public int ExpenseCategoryId { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal Amount { get; set; }

    [StringLength(3)]
    [Unicode(false)]
    public string CurrencyCode { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    public DateOnly ExpenseDate { get; set; }

    [Column("CreatedByUserID")]
    public int CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("Expenses")]
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("ExpenseCategoryId")]
    [InverseProperty("Expenses")]
    public virtual ExpenseCategory ExpenseCategory { get; set; } = null!;

    [ForeignKey("ShopId")]
    [InverseProperty("Expenses")]
    public virtual Shop Shop { get; set; } = null!;
}
