using System;
using System.Collections.Generic;

namespace RadarService.Entities.Models;

public partial class Scheduler
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public virtual ICollection<DeviceScheduler> DeviceSchedulers { get; } = new List<DeviceScheduler>();
}
