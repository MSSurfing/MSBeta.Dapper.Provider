using Autofac;
using Autofac.Engine;
using DapperExtensions;
using DapperExtensions.Connections;
using Grpc.Core;
using Grpc.Core.Interceptors;
using MSBeta.Dapper.Grpc5.Tests.Core.Infrastructure;
using MSBeta.Dapper.Grpc5.Tests.Implements;
using MSBeta.Dapper.Provider.Tests.Services;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using static Surfing.SurfService;

namespace MSBeta.Dapper.Grpc5.Tests
{
    public abstract class ImplementTests : BaseTests
    {
        private const string SERVICE_Host = "127.0.0.1";
        private const int SERVICE_PORT = 20389;

        protected ITestOutputHelper _testHelper = null;
        protected Channel LocalChannel;

        public ImplementTests()
        {
            LocalChannel = new Channel(SERVICE_Host, SERVICE_PORT, ChannelCredentials.Insecure);

            RunServer(SERVICE_PORT);
        }

        protected Channel GetChannel(string host = SERVICE_Host, bool useNew = false)
        {
            if (useNew)
                new Channel(SERVICE_Host, SERVICE_PORT, ChannelCredentials.Insecure);

            if (host == SERVICE_Host)
                return LocalChannel;

            return new Channel(host, SERVICE_PORT, ChannelCredentials.Insecure);
        }

        #region Attributes
        /// <summary>调试测试 开关</summary>
        protected bool IsDebug = false;
        #endregion

        #region Server
        private static object run_lock = new object();
        private static bool IsRunning = false;
        protected void RunServer(int port)
        {
            lock (run_lock)
            {
                if (IsRunning)
                    return;

                IsRunning = true;

                SurfProvider.Initialize();
                SurfProvider.OutputWriteLine = (e) => WriteLine(e);

                //EngineContext.Initialize();

                var server = ServerInitialize(port);

                Task.Run(() =>
                {
                    try
                    {
                        server.Start();
                        server.ShutdownTask.Wait();
                    }
                    catch { }
                });

                Thread.Sleep(300);
            }
        }

        private Server ServerInitialize(int port)
        {

            // Grpc service 
            var surfGService = Surfing.SurfService.BindService(SurfProvider.Container.Resolve<SurfServiceBase>());

            Server server = new Server()
            {
                Services = {
                    surfGService.Intercept(new  ServerCallContextInterceptor())
                },
                Ports = { new ServerPort("127.0.0.1", port, ServerCredentials.Insecure) }
            };

            return server;
        }

        public virtual void Dispose()
        {

        }
        #endregion

        #region Utilities
        protected virtual void WriteLine(string format, params object[] args)
        {
            if (_testHelper != null)
                _testHelper.WriteLine(format, args);

            System.Console.WriteLine(format, args);
            System.Diagnostics.Debug.WriteLine(format, args);
        }
        #endregion

        #region Register Dependency
        //protected void Register()
        //{
        //    var builder = new ContainerBuilder();

        //    var connectionString = "Data Source=139.159.137.99;Initial Catalog=Surfing.Test109;Persist Security Info=True;User ID=sa2;Password=z;Connect Timeout=360";
        //    var dbOption = DbOption.UseSqlServer;
        //    var connectionPool = DapperProvider.Initialize(connectionString, dbOption);
        //    //builder.Register<IConnectionPool>(e => connectionPool);

        //    builder.RegisterType<UserService>().As<IUserService>().InstancePerDependency();
        //    //builder.RegisterType<User2Service>().As<IUserService>().InstancePerDependency();

        //    builder.RegisterGeneric(typeof(DapperRepository<>)).As(typeof(IRepository<>))
        //        .InstancePerDependency();

        //    builder.RegisterType<SurfServiceImplement>().As<SurfServiceBase>().InstancePerDependency();

        //    var appContainer = builder.Build();

        //    var multitenantContainer = new MultitenantContainer()
        //}
        #endregion
    }
}
