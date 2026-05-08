using Microsoft.EntityFrameworkCore;
using Satur8.Domain.Models;

namespace Satur8.CoreApplication.Interfaces
{
    public interface ISaturatorDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Preset> Presets { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<Favourite> Favourites { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
