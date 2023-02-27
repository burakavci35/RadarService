using System;
using System.Collections.Generic;

namespace RadarService.Entities.Models;

public partial class ResponseCondition
{
    public int Id { get; set; }

    public int RequestId { get; set; }

    public string Condition { get; set; } = null!;

    public string Result { get; set; } = null!;

    public string RequestName { get; set; } = null!;

    public virtual Request Request { get; set; } = null!;
}
