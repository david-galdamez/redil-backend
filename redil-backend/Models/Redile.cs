using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace redil_backend.Models;

[Table("rediles")]
public partial class Redile
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(255)]
    public string? Description { get; set; }

    [Required, MaxLength(20)]
    public string Code { get; set; } = null!;

    [Required]
    public int NumCourse { get; set; }

    public ICollection<Class> Classes { get; set; } = new List<Class>();

    public ICollection<StudentRedil> StudentRedils { get; set; } = new List<StudentRedil>();

    public ICollection<User> Users { get; set; } = new List<User>();
}
