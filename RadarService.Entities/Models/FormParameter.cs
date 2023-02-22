using System;
using System.Collections.Generic;

namespace RadarService.Entities.Models;

public partial class FormParameter
{
    public int Id { get; set; }

    public int RequestId { get; set; }

    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual Request Request { get; set; } = null!;
}
