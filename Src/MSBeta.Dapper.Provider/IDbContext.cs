using System;
using System.Data.Common;

namespace DapperExtensions
{
    public interface IDbContext : IDisposable
    {
        /*
         * 
         * Todo ??
         * 
         */


        // temp test
        DbConnection GetDbConnection();
    }
}
