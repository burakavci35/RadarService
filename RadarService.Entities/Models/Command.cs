using System;
using System.Collections.Generic;

namespace RadarService.Entities.Models;

public partial class Command
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<DeviceCommand> DeviceCommands { get; } = new List<DeviceCommand>();

    public virtual ICollection<StepRequest> StepRequests { get; } = new List<StepRequest>();
}
