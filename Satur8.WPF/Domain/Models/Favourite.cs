using System;
using System.Collections.Generic;

namespace Satur8.WPF.Domain.Models;

public partial class Favourite
{
    public int FavouritesId { get; set; }

    public int UserId { get; set; }

    public int PresetId { get; set; }

    public virtual Preset Preset { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
