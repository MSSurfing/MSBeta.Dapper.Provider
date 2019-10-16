using Autofac;
using DapperExtensions;
using DapperExtensions.Connections;
using MSBeta.Dapper.Grpc5.Tests.Core.Infrastructure;
using MSBeta.Dapper.Grpc5.Tests.Core.Multitenant;
using MSBeta.Dapper.Grpc5.Tests.Implements;
using MSBeta.Dapper.Provider.Tests.Services;
using System;
using System.Diagnostics;
using static Surfing.SurfService;

namespace MSBeta.Dapper.Grpc5.Tests
{
    public class SurfProvider
    {
        public static MultitenantContainer Container { get; set; }
        public static StubTenantIdentificationStrategy TenantIdentifier;

        #region methods
        public static void Initialize()
        {
            TenantIdentifier = new StubTenantIdentificationStrategy();
            Container = Register();

        }

        public static void SetTenantId(object id)
        {
            TenantIdentifier.TenantId = id;
        }

        public static void DisposseTenant(object id)
        {
            Container.RemoveTenant(id);
            // Todo
        }
        #endregion

        public static Action<string> OutputWriteLine { get; set; }

        public static void WriteLine(string message)
        {
            OutputWriteLine?.Invoke(message);

            Debug.WriteLine(message);
        }



        #region Register Dependency
        protected static MultitenantContainer Register()
        {
            var builder = new ContainerBuilder();

            var connectionString = "Data Source=139.159.137.99;Initial Catalog=Surfing.Test109;Persist Security Info=True;User ID=sa2;Password=z;Connect Timeout=360";
            var dbOption = DbOption.UseSqlServer;
            var connectionPool = DapperProvider.Initialize(connectionString, dbOption);
            //builder.Register<IConnectionPool>(e => connectionPool);

            builder.RegisterType<UserService>().As<IUserService>().InstancePerDependency()
                .OnRelease(e =>
                {
                    WriteLine("Release IUserService");
                });
            //builder.RegisterType<User2Service>().As<IUserService>().InstancePerDependency();

            builder.RegisterGeneric(typeof(DapperRepository<>)).As(typeof(IRepository<>))
                .InstancePerDependency()
                .OnRelease(e =>
                {
                    WriteLine("Release IRepository");
                });

            builder.RegisterType<SurfServiceImplement>().As<SurfServiceBase>().InstancePerDependency();

            var appContainer = builder.Build();

            var multitenantContainer = new MultitenantContainer(TenantIdentifier, appContainer);

            return multitenantContainer;
        }
        #endregion
    }
}
