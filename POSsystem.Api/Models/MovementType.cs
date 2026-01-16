using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POSsystem.Api.Models;

[Table("MovementType")]
[Index("Code", Name = "UQ__Movement__A25C5AA7B65E68F6", IsUnique = true)]
public partial class MovementType
{
    [Key]
    [Column("MovementTypeID")]
    public int MovementTypeId { get; set; }

    [StringLength(50)]
    public string Code { get; set; } = null!;

    [StringLength(200)]
    public string? Description { get; set; }

    [InverseProperty("MovementType")]
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
