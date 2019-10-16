using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Grpc.Core;
using MSBeta.Dapper.Provider.Tests.Services;
using Surfing;

namespace MSBeta.Dapper.Grpc5.Tests.Implements
{
    public class SurfServiceImplement : Surfing.SurfService.SurfServiceBase
    {
        #region Fields
        #endregion

        public SurfServiceImplement()
        {

        }

        public override Task<GetRes> GetOne(GetReq request, ServerCallContext context)
        {
            var guid = Guid.NewGuid();

            var list = SurfProvider.Container.Resolve<IUserService>().Search(pageSize: 1);

            var entity = list.FirstOrDefault();

            return Task.FromResult(new GetRes() { Data = entity == null ? "" : entity.Id.ToString() });
        }
    }
}
