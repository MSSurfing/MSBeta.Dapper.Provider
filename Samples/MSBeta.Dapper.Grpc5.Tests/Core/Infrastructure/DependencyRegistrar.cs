// 暂不用 Autofac.Engine

//using Autofac;
//using Autofac.Engine;
//using DapperExtensions;
//using DapperExtensions.Connections;
//using MSBeta.Dapper.Grpc5.Tests.Implements;
//using MSBeta.Dapper.Provider.Tests.Services;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using static Surfing.SurfService;

//namespace MSBeta.Dapper.Grpc5.Tests.Core.Infrastructure
//{
//    public class DependencyRegistrar : IDependencyRegistrar
//    {
//        public int Order => 1;

//        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
//        {
//            var connectionString = "Data Source=139.159.137.99;Initial Catalog=Surfing.Test109;Persist Security Info=True;User ID=sa2;Password=z;Connect Timeout=360";
//            var dbOption = DbOption.UseSqlServer;
//            var connectionPool = DapperProvider.Initialize(connectionString, dbOption);
//            //builder.Register<IConnectionPool>(e => connectionPool);

//            //builder.RegisterType<UserService>().As<IUserService>().InstancePerDependency();
//            builder.RegisterType<User2Service>().As<IUserService>().InstancePerDependency();

//            builder.RegisterGeneric(typeof(DapperRepository<>)).As(typeof(IRepository<>))
//                .InstancePerDependency();

//            builder.RegisterType<SurfServiceImplement>().As<SurfServiceBase>().InstancePerDependency();
//        }
//    }
//}
