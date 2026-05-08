using System;
using System.Collections.Generic;

namespace Satur8.Domain.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Preset> Presets { get; set; } = new List<Preset>();
}
