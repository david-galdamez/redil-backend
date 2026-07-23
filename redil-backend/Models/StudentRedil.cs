using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace redil_backend.Models;

[Table("student_redil")]
public partial class StudentRedil
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    [Required]
    public int RedilId { get; set; }
    public Redile Redil { get; set; } = null!;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public bool Active { get; set; } = true;


}
