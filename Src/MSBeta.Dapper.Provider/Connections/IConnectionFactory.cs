using System.Data.Common;

namespace DapperExtensions.Connections
{
    public interface IConnectionFactory
    {
        DbOption DbOption { get; }

        DbConnection CreateConnection(string connectionString);
    }
}