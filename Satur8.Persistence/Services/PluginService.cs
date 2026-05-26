using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Satur8.Persistence.Services
{
    public static class PluginService
    {
        public static IServiceProvider Services { get; private set; } = null!;

        public static void Initialize()
        {
            if (Services != null)
            {
                return;
            }

            var basePath = Path.GetDirectoryName(typeof(PluginService).Assembly.Location)!;

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            var services = new ServiceCollection();

            services.AddPersistence(configuration);

            Services = services.BuildServiceProvider();

            using (var scope = Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<SaturatorDbContext>();

                DbInitializer.Initialize(db);
            }
        }
    }
}
