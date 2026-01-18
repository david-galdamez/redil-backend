using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace redil_backend.Models;

[Table("class_details")]
public partial class ClassDetail
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ClassId { get; set; }
    public Class Class { get; set; } = null!;

    [Required]
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public bool Attendance { get; set; }


}
