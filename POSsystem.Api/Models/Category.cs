using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POSsystem.Api.Models;

[Table("Category")]
[Index("ShopId", "Name", Name = "UQ_Category_Shop_Name", IsUnique = true)]
public partial class Category
{
    [Key]
    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [Column("ShopID")]
    public int ShopId { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [ForeignKey("ShopId")]
    [InverseProperty("Categories")]
    public virtual Shop Shop { get; set; } = null!;
}
