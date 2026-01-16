using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POSsystem.Api.Models;

[Table("ExpenseCategory")]
[Index("Name", Name = "UQ_ExpenseCategory_Name", IsUnique = true)]
public partial class ExpenseCategory
{
    [Key]
    [Column("ExpenseCategoryID")]
    public int ExpenseCategoryId { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;

    [InverseProperty("ExpenseCategory")]
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
