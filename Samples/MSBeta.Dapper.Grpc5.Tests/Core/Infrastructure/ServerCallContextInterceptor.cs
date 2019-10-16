using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MSBeta.Dapper.Grpc5.Tests.Core.Infrastructure
{
    public class ServerCallContextInterceptor : Interceptor
    {
        #region Fields
        //private readonly 
        #endregion

        #region Constructor
        public ServerCallContextInterceptor()
        {

        }
        #endregion

        #region Utilities
        #endregion

        #region Override Methods
        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var t = Thread.CurrentThread.ManagedThreadId;
            var id = "Surf-g" + t;
            SurfProvider.SetTenantId(id);
            SurfProvider.Container.ConfigureTenant(id, e => { });
            try
            {
                return continuation(request, context);
            }
            finally { SurfProvider.DisposseTenant(id); }
        }
        #endregion
    }
}
