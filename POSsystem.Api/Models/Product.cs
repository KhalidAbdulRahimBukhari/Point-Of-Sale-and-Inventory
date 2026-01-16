using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POSsystem.Api.Models;

[Table("Product")]
public partial class Product
{
    [Key]
    [Column("ProductID")]
    public int ProductId { get; set; }

    [Column("ShopID")]
    public int ShopId { get; set; }

    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? ImagePath { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    [StringLength(100)]
    public string? Brand { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

    [ForeignKey("ShopId")]
    [InverseProperty("Products")]
    public virtual Shop Shop { get; set; } = null!;
}
