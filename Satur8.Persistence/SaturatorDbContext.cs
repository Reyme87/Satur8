using Microsoft.EntityFrameworkCore;
using Satur8.Domain.Models;
using Satur8.Persistence.EntityTypeConfiguration;
using Satur8.CoreApplication.Interfaces;

namespace Satur8.Persistence
{
    public class SaturatorDbContext : DbContext, ISaturatorDbContext
    {
        public SaturatorDbContext(DbContextOptions<SaturatorDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Preset> Presets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Favourite> Favourites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new PresetConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new FavouriteConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
