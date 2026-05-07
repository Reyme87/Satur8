using Microsoft.EntityFrameworkCore;
using Satur8.WPF.Domain.Models;

namespace Satur8.WPF.Infrastructure.Interfaces
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
