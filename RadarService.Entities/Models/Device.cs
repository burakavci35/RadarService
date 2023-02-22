using System;
using System.Collections.Generic;

namespace RadarService.Entities.Models;

public partial class Device
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string BaseAddress { get; set; } = null!;

    public string Status { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<DeviceCommand> DeviceCommands { get; } = new List<DeviceCommand>();

    public virtual ICollection<DeviceScheduler> DeviceSchedulers { get; } = new List<DeviceScheduler>();
}
