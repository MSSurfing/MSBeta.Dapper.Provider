using Autofac.Engine;
using DapperExtensions;
using MSBeta.Dapper.Provider.Tests.Data.Entity;
using MSBeta.Dapper.Provider.Tests.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MSBeta.Dapper.Provider.Sample.Tests
{
    public class RepositoryTests : BaseTests
    {
        [Fact]
        public virtual void debug_service_tests()
        {
            var userService = EngineContext.Resolve<IUserService>();

            userService.Insert(new User { Id = Guid.NewGuid(), Name = "Mock" });

            var users = userService.Search();
        }

        [Fact]
        public virtual void debug_services_tests()
        {
            EngineContext.Scope.Resolve<IUserService>().Insert(new User { Id = Guid.NewGuid(), Name = "Mock02" });

            int toExclusive = 10000;
            Parallel.For(1, toExclusive, index =>
            {
                var userService = EngineContext.Scope.Resolve<IUserService>();
                var users = userService.Search();

                userService.Dispose();
            });
        }

        [Fact]
        public virtual void debug_services_tests2()
        {
            //EngineContext.Resolve<IUserService>().Insert(new User { Id = Guid.NewGuid(), Name = "Mock02" });

            int toExclusive = 3000;
            for (int i = 0; i < toExclusive; i++)
            {
                using (var userService = EngineContext.Resolve<IRepository<User>>())
                {
                    var users = userService.GetOne();
                }
            };

        }

        [Fact]
        public virtual void debug_services_tests3()
        {
            //EngineContext.Resolve<IUserService>().Insert(new User { Id = Guid.NewGuid(), Name = "Mock02" });

            int toExclusive = 1000;
            for (int i = 0; i < toExclusive; i++)
            {
                var userService = new DapperRepository<User>();
                var users = userService.GetOne();
            };
        }
    }
}
