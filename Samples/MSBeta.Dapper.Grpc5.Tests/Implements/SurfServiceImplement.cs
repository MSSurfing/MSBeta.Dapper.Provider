using System;
using System.Threading.Tasks;
using Grpc.Core;
using Surfing;

namespace MSBeta.Dapper.Grpc5.Tests.Implements
{
    public class SurfServiceImplement : Surfing.SurfService.SurfServiceBase
    {
        public override Task<GetRes> GetOne(GetReq request, ServerCallContext context)
        {

            return Task.FromResult(new GetRes() { Data = Guid.NewGuid().ToString() });
        }
    }
}
