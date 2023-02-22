using System;
using System.Collections.Generic;

namespace RadarService.Entities.Models;

public partial class Step
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<StepRequest> StepRequests { get; } = new List<StepRequest>();
}
