namespace Satur8.WPF.Infrastructure
{
    public static class DbInitializer
    {
        public static void Initialize(SaturatorDbContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
