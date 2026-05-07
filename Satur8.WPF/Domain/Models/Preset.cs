using System;
using System.Collections.Generic;

namespace Satur8.WPF.Domain.Models;

public partial class Preset
{
    public int PresetId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public int UserId { get; set; }

    public string ParametersJson { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();

    public virtual User User { get; set; } = null!;
}
