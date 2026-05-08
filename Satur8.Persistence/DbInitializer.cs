namespace Satur8.Persistence
{
    public static class DbInitializer
    {
        public static void Initialize(SaturatorDbContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
