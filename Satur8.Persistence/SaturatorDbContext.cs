using Microsoft.EntityFrameworkCore;
using Satur8.WPF.Domain.Models;
using Satur8.WPF.Infrastructure.EntityTypeConfiguration;
using Satur8.WPF.Infrastructure.Interfaces;

namespace Satur8.WPF.Infrastructure
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
