using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace DapperExtensions.Connections
{
    public class EmptyConnectionPool : IConnectionPool
    {
        public string ConnectionString => string.Empty;

        public void Clear()
        {

        }

        public void Dispose()
        {

        }

        public void Initialize()
        {

        }

        public DbConnection RentConnection()
        {
            return null;
        }
    }
}
