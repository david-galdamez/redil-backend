using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace redil_backend.Models;

[Table("classes")]
public partial class Class
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int RedilId { get; set; }
    public Redile Redil { get; set; } = null!;

    [Required]
    public int TeacherId { get; set; }
    public User Teacher { get; set; } = null!;

    [Required]
    public DateTime ClassDate { get; set; }

    [Required]
    public string ClassDescription { get; set; } = null!;

    public string? AttendanceToken { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public ICollection<ClassDetail> ClassDetails { get; set; } = new List<ClassDetail>();
}
