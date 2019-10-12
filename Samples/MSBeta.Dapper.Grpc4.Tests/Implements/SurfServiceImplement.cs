using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using MSBeta.Dapper.Provider.Tests.Services;
using Surfing;

namespace MSBeta.Dapper.Grpc4.Tests.Implements
{
    public class SurfServiceImplement : Surfing.SurfService.SurfServiceBase
    {
        #region Fields
        private readonly IUserService _userService;
        #endregion

        public SurfServiceImplement()
        {
            _userService = new UserService();
        }

        public override Task<GetRes> GetOne(GetReq request, ServerCallContext context)
        {
            var guid = Guid.NewGuid();

            var list = _userService.Search(pageSize: 1);

            var entity = list.FirstOrDefault();

            return Task.FromResult(new GetRes() { Data = entity == null ? "" : entity.Id.ToString() });
        }
    }
}
