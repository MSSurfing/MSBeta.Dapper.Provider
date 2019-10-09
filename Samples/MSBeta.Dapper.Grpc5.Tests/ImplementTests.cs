using Grpc.Core;
using MSBeta.Dapper.Grpc5.Tests.Implements;
using System.Threading;
using System.Threading.Tasks;

namespace MSBeta.Dapper.Grpc5.Tests
{
    public abstract class ImplementTests
    {
        private const string SERVICE_Host = "127.0.0.1";
        private const int SERVICE_PORT = 20389;

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

                //EngineContext.Initialize();

                Task.Run(() =>
                {
                    try
                    {
                        var server = ServerInitialize(port);
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
            var roleGService = Surfing.SurfService.BindService(new SurfServiceImplement());

            Server server = new Server()
            {
                Services = {
                    roleGService
                },
                Ports = { new ServerPort("127.0.0.1", port, ServerCredentials.Insecure) }
            };

            return server;
        }

        public virtual void Dispose()
        {

        }
        #endregion
    }
}
