using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Satur8.CoreApplication.Interfaces;

namespace Satur8.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<SaturatorDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
            services.AddScoped<ISaturatorDbContext>(provider => provider.GetRequiredService<SaturatorDbContext>());

            return services;
        }
    }
}
