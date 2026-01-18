using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace redil_backend.Models;

[Table("students")]
public partial class Student
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(100)]
    public string Email { get; set; } = null!;

    public bool IsServer { get; set; }

    [Required]
    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<ClassDetail> ClassDetails { get; set; } = new List<ClassDetail>();


    public ICollection<StudentRedil> StudentRedils { get; set; } = new List<StudentRedil>();
}
