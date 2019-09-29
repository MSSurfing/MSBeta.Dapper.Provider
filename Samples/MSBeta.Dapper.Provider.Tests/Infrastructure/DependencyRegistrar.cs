using Autofac;
using Autofac.Engine;
using DapperExtensions;
using DapperExtensions.Connections;
using MSBeta.Dapper.Provider.Tests.Services;

namespace MSBeta.Dapper.Provider.Sample.Tests.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 9;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            var connectionString = "Data Source=139.159.137.99;Initial Catalog=Surfing.Test109;Persist Security Info=True;User ID=sa2;Password=z;Connect Timeout=360";
            var dbOption = DbOption.UseSqlServer;

            DapperProvider.Initialize(connectionString, dbOption);

            builder.RegisterType<UserService>().As<IUserService>().InstancePerDependency();

            builder.RegisterGeneric(typeof(DapperRepository<>)).As(typeof(IRepository<>)).InstancePerDependency();
        }
    }
}
