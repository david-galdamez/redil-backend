using System;
using System.Collections.Generic;

namespace redil_backend.Models;

public partial class roles
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public virtual ICollection<users> users { get; set; } = new List<users>();
}
