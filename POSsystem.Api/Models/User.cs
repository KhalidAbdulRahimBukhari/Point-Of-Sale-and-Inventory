using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POSsystem.Api.Models;

[Index("UserName", Name = "UQ_Users_UserName", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [Column("PersonID")]
    public int PersonId { get; set; }

    [Column("ShopID")]
    public int ShopId { get; set; }

    [Column("RoleID")]
    public int RoleId { get; set; }

    [StringLength(100)]
    public string UserName { get; set; } = null!;

    [StringLength(256)]
    public string Password { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    [ForeignKey("PersonId")]
    [InverseProperty("Users")]
    public virtual Person Person { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<SaleReturn> SaleReturns { get; set; } = new List<SaleReturn>();

    [InverseProperty("User")]
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    [ForeignKey("ShopId")]
    [InverseProperty("Users")]
    public virtual Shop Shop { get; set; } = null!;

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
