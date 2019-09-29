using System.Data;
using DapperExtensions.Connections;

namespace DapperExtensions
{
    public class DbContextPoolable : IDbContext
    {
        #region Fields
        private IConnectionPool _connectionPool;
        #endregion

        public DbContextPoolable(IConnectionPool connectionPool)
        {
            _connectionPool = connectionPool;
        }

        public IDbConnection GetDbConnection()
        {
            return _connectionPool.RentConnection();
        }

        public void Dispose()
        {
            // Todo
        }
    }
}
