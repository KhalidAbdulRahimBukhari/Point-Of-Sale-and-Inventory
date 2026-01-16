using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace POSsystem.Api.Models;

[Table("SchemaVersion")]
public partial class SchemaVersion
{
    [Key]
    public int Version { get; set; }

    public DateTime AppliedAt { get; set; }
}
