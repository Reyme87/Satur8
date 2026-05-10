using System;
using System.Collections.Generic;

namespace Satur8.Domain.Models;

public class Favourite
{
    public Guid FavouritesId { get; set; }

    public Guid UserId { get; set; }

    public Guid PresetId { get; set; }

    public virtual Preset Preset { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
