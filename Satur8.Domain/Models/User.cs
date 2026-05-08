using System;
using System.Collections.Generic;

namespace Satur8.Domain.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Login { get; set; }

    public string? PasswordHash { get; set; }

    public virtual ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();

    public virtual ICollection<Preset> Presets { get; set; } = new List<Preset>();
}
