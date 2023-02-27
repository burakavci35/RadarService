using System;
using System.Collections.Generic;

namespace RadarService.Entities.Models;

public partial class DeviceRequest
{
    public int Id { get; set; }

    public int DeviceId { get; set; }

    public int RequestId { get; set; }

    public virtual Device Device { get; set; } = null!;

    public virtual Request Request { get; set; } = null!;
}
