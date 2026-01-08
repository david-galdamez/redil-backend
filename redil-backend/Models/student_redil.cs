using System;
using System.Collections.Generic;

namespace redil_backend.Models;

public partial class student_redil
{
    public int id { get; set; }

    public int student_id { get; set; }

    public int redil_id { get; set; }

    public DateTime joined_at { get; set; }

    public bool active { get; set; }

    public virtual rediles redil { get; set; } = null!;

    public virtual students student { get; set; } = null!;
}
