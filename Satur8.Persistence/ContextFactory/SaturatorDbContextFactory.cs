using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Satur8.Persistence.ContextFactory
{
    public class SaturatorDbContextFactory : IDesignTimeDbContextFactory<SaturatorDbContext>
    {
        public SaturatorDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "../Satur8.WPF");

            IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();

            var connectionString =
            configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder =
                new DbContextOptionsBuilder<SaturatorDbContext>();

            optionsBuilder.UseNpgsql(connectionString);

            return new SaturatorDbContext(optionsBuilder.Options);
        }
    }
}
