using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace redil_backend.Models;

[Table("users")]
public partial class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(100)]
    public string Email { get; set; } = null!;

    [Required, MaxLength(255)]
    public string Password { get; set; } = null!;

    public bool IsActive { get; set; }

    [Required]
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public int? RedilId { get; set; }
    public Redile? Redil { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<Class> Classes { get; set; } = new List<Class>();


}
