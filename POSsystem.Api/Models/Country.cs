using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POSsystem.Api.Models;

[Table("Country")]
public partial class Country
{
    [Key]
    [Column("CountryID")]
    public int CountryId { get; set; }

    [StringLength(100)]
    public string CountryName { get; set; } = null!;

    [InverseProperty("Country")]
    public virtual ICollection<Person> People { get; set; } = new List<Person>();

    [InverseProperty("Country")]
    public virtual ICollection<Shop> Shops { get; set; } = new List<Shop>();
}
