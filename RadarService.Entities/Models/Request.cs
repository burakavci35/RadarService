using System;
using System.Collections.Generic;

namespace RadarService.Entities.Models;

public partial class Request
{
    public int Id { get; set; }

    public string Url { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Response { get; set; }

    public virtual ICollection<FormParameter> FormParameters { get; } = new List<FormParameter>();

    public virtual ICollection<ResponseCondition> ResponseConditions { get; } = new List<ResponseCondition>();

    public virtual ICollection<StepRequest> StepRequests { get; } = new List<StepRequest>();
}
