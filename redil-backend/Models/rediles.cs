using System;
using System.Collections.Generic;

namespace redil_backend.Models;

public partial class rediles
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public virtual ICollection<classes> classes { get; set; } = new List<classes>();

    public virtual ICollection<student_redil> student_redil { get; set; } = new List<student_redil>();

    public virtual ICollection<users> users { get; set; } = new List<users>();
}
