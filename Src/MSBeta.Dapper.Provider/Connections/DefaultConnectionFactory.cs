using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Data.SqlClient;

namespace DapperExtensions.Connections
{
    public class DefaultConnectionFactory : IConnectionFactory
    {
        private readonly DbOption _dbOption;
        public DefaultConnectionFactory(DbOption dbOption = DbOption.UseSqlServer)
        {
            _dbOption = dbOption;
        }

        public DbOption DbOption => _dbOption;

        public DbConnection CreateConnection(string connectionString)
        {
            DbConnection dbConnection;
            switch (_dbOption)
            {
                case DbOption.UseMySql:
                    dbConnection = new MySqlConnection(connectionString);
                    break;

                case DbOption.UseSqlServer:
                default:
                    dbConnection = new SqlConnection(connectionString);
                    break;
            }

            return dbConnection;
        }
    }
}
