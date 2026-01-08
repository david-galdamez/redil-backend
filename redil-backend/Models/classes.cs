using System;
using System.Collections.Generic;

namespace redil_backend.Models;

public partial class classes
{
    public int id { get; set; }

    public int redil_id { get; set; }

    public int teacher_id { get; set; }

    public DateTime class_date { get; set; }

    public string class_description { get; set; } = null!;

    public string? attendance_token { get; set; }

    public DateTime? expires_at { get; set; }

    public virtual ICollection<class_details> class_details { get; set; } = new List<class_details>();

    public virtual rediles redil { get; set; } = null!;

    public virtual users teacher { get; set; } = null!;
}
