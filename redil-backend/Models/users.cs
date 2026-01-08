using System;
using System.Collections.Generic;

namespace redil_backend.Models;

public partial class users
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public string email { get; set; } = null!;

    public string password { get; set; } = null!;

    public bool? is_active { get; set; }

    public int role_id { get; set; }

    public int? redil_id { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<classes> classes { get; set; } = new List<classes>();

    public virtual rediles? redil { get; set; }

    public virtual roles role { get; set; } = null!;
}
