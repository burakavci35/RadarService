using System;
using System.Collections.Generic;

namespace RadarService.Entities.Models;

public partial class DeviceScheduler
{
    public int Id { get; set; }

    public int SchedulerId { get; set; }

    public int DeviceId { get; set; }

    public virtual Device Device { get; set; } = null!;

    public virtual Scheduler Scheduler { get; set; } = null!;
}
