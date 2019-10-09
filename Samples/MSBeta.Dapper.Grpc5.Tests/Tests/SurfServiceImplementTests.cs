using Surfing;
using System;
using Xunit;
using static Surfing.SurfService;

namespace MSBeta.Dapper.Grpc5.Tests
{
    public class SurfServiceImplementTests : ImplementTests
    {
        #region Fields & Consts
        private readonly SurfServiceClient _client;
        #endregion

        public SurfServiceImplementTests()
        {
            _client = new SurfServiceClient(base.LocalChannel);
        }

        [Fact]
        public void get_one_test()
        {
            var res = _client.GetOne(new GetReq { Id = Guid.NewGuid().ToString() });
        }
    }
}
