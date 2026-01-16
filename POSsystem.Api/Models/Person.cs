using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POSsystem.Api.Models;

[Table("Person")]
[Index("NationalNumber", Name = "UQ_Person_NationalNumber", IsUnique = true)]
public partial class Person
{
    [Key]
    [Column("PersonID")]
    public int PersonId { get; set; }

    [StringLength(200)]
    public string FullName { get; set; } = null!;

    [StringLength(50)]
    public string? NationalNumber { get; set; }

    [StringLength(200)]
    public string? Email { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [Column("CountryID")]
    public int? CountryId { get; set; }

    [StringLength(10)]
    public string? Gender { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(500)]
    public string? ImagePath { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("CountryId")]
    [InverseProperty("People")]
    public virtual Country? Country { get; set; }

    [InverseProperty("Person")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
