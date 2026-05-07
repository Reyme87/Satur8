using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Windows;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Satur8.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            var services = new ServiceCollection();

            services.AddDbContext<SaturatorContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            );

            services.AddSingleton<MainWindow>();

            ServiceProvider = services.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();

            base.OnStartup(e);
        }
    }

}
