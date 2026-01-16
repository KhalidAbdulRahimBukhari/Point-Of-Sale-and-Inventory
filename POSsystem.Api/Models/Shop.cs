using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POSsystem.Api.Models;

[Table("Shop")]
public partial class Shop
{
    [Key]
    [Column("ShopID")]
    public int ShopId { get; set; }

    [Column("CountryID")]
    public int CountryId { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(3)]
    [Unicode(false)]
    public string CurrencyCode { get; set; } = null!;

    [StringLength(100)]
    public string City { get; set; } = null!;

    [StringLength(100)]
    public string? District { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    [InverseProperty("Shop")]
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    [ForeignKey("CountryId")]
    [InverseProperty("Shops")]
    public virtual Country Country { get; set; } = null!;

    [InverseProperty("Shop")]
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    [InverseProperty("Shop")]
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

    [InverseProperty("Shop")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [InverseProperty("Shop")]
    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

    [InverseProperty("Shop")]
    public virtual ICollection<SaleReturn> SaleReturns { get; set; } = new List<SaleReturn>();

    [InverseProperty("Shop")]
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    [InverseProperty("Shop")]
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

    [InverseProperty("Shop")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
