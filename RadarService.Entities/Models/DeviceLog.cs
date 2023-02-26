using System;
using System.Collections.Generic;

namespace RadarService.Entities.Models;

public partial class DeviceLog
{
    public int Id { get; set; }

    public int DeviceId { get; set; }

    public string Type { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime LogDateTime { get; set; }

    public virtual Device Device { get; set; } = null!;
}
