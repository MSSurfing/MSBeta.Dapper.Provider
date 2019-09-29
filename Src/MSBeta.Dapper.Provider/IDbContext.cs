using System;
using System.Data;

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
        IDbConnection GetDbConnection();
    }
}
