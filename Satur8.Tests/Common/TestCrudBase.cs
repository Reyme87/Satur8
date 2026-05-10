using Satur8.Persistence;

namespace Satur8.Tests.Common
{
    public class TestCrudBase : IDisposable
    {
        protected readonly SaturatorDbContext Context;

        public TestCrudBase()
        {
            Context = SaturatorContextFactory.Create();
        }

        public void Dispose()
        {
            SaturatorContextFactory.Destroy(Context);
        }
    }
}
