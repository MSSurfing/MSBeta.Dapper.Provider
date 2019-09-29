using DapperExtensions.Internal;
using System;
using System.Data;
using System.Data.Common;

namespace DapperExtensions.Connections.Internal
{
    internal sealed class RentedConnection : DbConnection, IDisposable
    {
        private ConnectionPool _connectionPool;
        private bool _disposed;
        private ReferenceCounted<PooledConnection> _reference;

        public RentedConnection(ConnectionPool connectionPool, ReferenceCounted<PooledConnection> reference)
        {
            _connectionPool = connectionPool;
            _reference = reference;
        }

        public override string ConnectionString { get => _reference.Instance.ConnectionString; set => _reference.Instance.ConnectionString = value; }

        public override int ConnectionTimeout => _reference.Instance.ConnectionTimeout;

        public override string Database => _reference.Instance.Database;

        public override ConnectionState State => _reference.Instance.State;

        public override string DataSource => _reference.Instance.DataSource;

        public override string ServerVersion => _reference.Instance.ServerVersion;

        public DbTransaction BeginTransaction()
        {
            return _reference.Instance.BeginTransaction();
        }

        public DbTransaction BeginTransaction(IsolationLevel il)
        {
            return _reference.Instance.BeginTransaction(il);
        }

        public override void ChangeDatabase(string databaseName)
        {
            _reference.Instance.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            _reference.Instance.Close();
        }

        public IDbCommand CreateCommand()
        {
            return _reference.Instance.CreateCommand();
        }

        public new void Dispose()
        {
            if (!_disposed)
            {
                _reference.DecrementReferenceCount();
                _disposed = true;
            }
        }

        public override void Open()
        {
            _reference.Instance.Open();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return _reference.Instance.BeginTransaction(isolationLevel);
        }

        protected override DbCommand CreateDbCommand()
        {
            return _reference.Instance.CreateCommand();
        }
    }
}
