using System;
using System.Collections.Generic;

namespace RadarService.Entities.Models;

public partial class DeviceCommand
{
    public int Id { get; set; }

    public int DeviceId { get; set; }

    public int CommandId { get; set; }

    public virtual Command Command { get; set; } = null!;

    public virtual Device Device { get; set; } = null!;
}
