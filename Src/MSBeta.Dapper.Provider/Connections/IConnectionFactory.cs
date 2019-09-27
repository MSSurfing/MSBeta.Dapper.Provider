using System.Data.Common;

namespace DapperExtensions.Connections
{
    public interface IConnectionFactory
    {
        DbConnection CreateConnection(string connectionString);
    }
}