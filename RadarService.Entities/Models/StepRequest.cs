using System;
using System.Collections.Generic;

namespace RadarService.Entities.Models;

public partial class StepRequest
{
    public int Id { get; set; }

    public int StepId { get; set; }

    public int RequestId { get; set; }

    public int CommandId { get; set; }

    public virtual Command Command { get; set; } = null!;

    public virtual Request Request { get; set; } = null!;

    public virtual Step Step { get; set; } = null!;
}
