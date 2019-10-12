using Surfing;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static Surfing.SurfService;

namespace MSBeta.Dapper.Grpc5.Tests
{
    public class SurfServiceImplementTests : ImplementTests
    {
        #region Fields & Consts
        private readonly SurfServiceClient _client;
        #endregion

        public SurfServiceImplementTests(ITestOutputHelper testHelper)
        {
            base._testHelper = testHelper;
            _client = new SurfServiceClient(base.LocalChannel);
        }

        [Fact]
        public void get_one_test()
        {
            var res = _client.GetOne(new GetReq { Id = Guid.NewGuid().ToString() });
        }

        [Fact]
        public void parallel_test()
        {
            int requestTime = 100000;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            int i = 0;
            Parallel.For(1, requestTime, index =>
            {
                var res = _client.GetOne(new GetReq { Id = Guid.NewGuid().ToString() });
                i++;
            });

            sw.Stop();
            WriteLine("同步完成 耗时：{0}量,{1}s", i, sw.ElapsedMilliseconds);
        }
    }
}
