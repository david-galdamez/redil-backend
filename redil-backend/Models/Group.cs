using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace redil_backend.Models;


[Table("groups")]
public partial class Group
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    public ICollection<Student> Students { get; set; } = new List<Student>();
}
