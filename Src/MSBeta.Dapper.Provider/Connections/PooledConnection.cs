using System;
using System.Data;
using System.Data.Common;

namespace DapperExtensions.Connections
{
    public class PooledConnection : IDisposable
    {
        private readonly DbConnection _dbConnection;
        private readonly ConnectionPool _connectionPool;
        private readonly int _version;

        public PooledConnection(ConnectionPool connectionPool, DbConnection dbConnection)
        {
            _connectionPool = connectionPool;
            _dbConnection = dbConnection;
            _version = connectionPool.Version;
        }

        public bool IsExpired
        {
            get
            {
                return _version < _connectionPool.Version || _dbConnection.State == ConnectionState.Closed;
            }
        }


        public string ConnectionString { get => _dbConnection.ConnectionString; set => _dbConnection.ConnectionString = value; }

        public int ConnectionTimeout => _dbConnection.ConnectionTimeout;

        public string Database => _dbConnection.Database;

        public string DataSource => _dbConnection.DataSource;

        public string ServerVersion => _dbConnection.ServerVersion;

        public ConnectionState State => _dbConnection.State;

        public DbTransaction BeginTransaction()
        {
            return _dbConnection.BeginTransaction();
        }

        public DbTransaction BeginTransaction(IsolationLevel il)
        {
            return _dbConnection.BeginTransaction(il);
        }

        public void ChangeDatabase(string databaseName)
        {
            _dbConnection.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            _dbConnection.Close();
        }

        public DbCommand CreateCommand()
        {
            return _dbConnection.CreateCommand();
        }

        public void Dispose()
        {
            _dbConnection.Dispose();
        }

        public void Open()
        {
            _dbConnection.Open();
        }
    }
}
