using System;
using System.Collections.Generic;

namespace Satur8.Domain.Models;

public class Preset
{
    public Guid PresetId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public Guid CategoryId { get; set; }

    public Guid UserId { get; set; }

    public string ParametersJson { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();

    public virtual User User { get; set; } = null!;
}
