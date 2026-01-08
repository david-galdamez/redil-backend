using System;
using System.Collections.Generic;

namespace redil_backend.Models;

public partial class class_details
{
    public int id { get; set; }

    public int class_id { get; set; }

    public int student_id { get; set; }

    public bool attendance { get; set; }

    public virtual classes _class { get; set; } = null!;

    public virtual students student { get; set; } = null!;
}
