using System;
using System.Collections.Generic;

namespace redil_backend.Models;

public partial class students
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public string email { get; set; } = null!;

    public bool is_server { get; set; }

    public int group_id { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<class_details> class_details { get; set; } = new List<class_details>();

    public virtual groups group { get; set; } = null!;

    public virtual ICollection<student_redil> student_redil { get; set; } = new List<student_redil>();
}
