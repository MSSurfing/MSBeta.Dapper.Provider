using Autofac.Engine;

namespace MSBeta.Dapper.Provider.Sample.Tests
{
    public abstract class BaseTests
    {
        public BaseTests()
        {
            EngineContext.Initialize();
        }
    }
}
