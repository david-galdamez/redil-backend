using System;
using System.Collections.Generic;

namespace redil_backend.Models;

public partial class groups
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public virtual ICollection<students> students { get; set; } = new List<students>();
}
