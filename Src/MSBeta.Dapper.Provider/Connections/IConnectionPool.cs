using System;
using System.Data.Common;

namespace DapperExtensions.Connections
{
    public interface IConnectionPool : IDisposable
    {
        string ConnectionString { get; }

        DbConnection RentConnection();

        void Clear();

        void Initialize();
    }
}
