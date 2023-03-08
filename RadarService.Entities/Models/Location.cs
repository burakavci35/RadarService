using System;
using System.Collections.Generic;

namespace RadarService.Entities.Models;

public partial class Location
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Device> Devices { get; } = new List<Device>();
}
