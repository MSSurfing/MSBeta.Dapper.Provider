using DapperExtensions.Configuration;
using DapperExtensions.Connections.Internal;
using DapperExtensions.Internal;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DapperExtensions.Connections
{
    public partial class ConnectionPool : IConnectionPool
    {
        #region Fields
        private readonly ConnectionOptions _options;
        private readonly IConnectionFactory _connectionFactory;
        private readonly string _connectionString;
        private int _version;
        private readonly CancellationTokenSource _maintenanceCancellationTokenSource;
        private readonly InterlockedInt32 _state;

        private readonly SemaphoreSlim _poolQueue;
        internal SemaphoreSlim WaitQueue { get; private set; }

        internal ListConnectionHolder ListConnectionHolder { get; private set; }
        #endregion

        #region Ctor
        public ConnectionPool(string connectionString
            , ConnectionOptions options
            , IConnectionFactory connectionFactory)
        {
            _connectionString = connectionString;
            _options = options;
            _connectionFactory = connectionFactory;

            ListConnectionHolder = new ListConnectionHolder();
            _poolQueue = new SemaphoreSlim(options.MaxConnections);
            WaitQueue = new SemaphoreSlim(options.WaitQueueSize);
            _state = new InterlockedInt32(ConnectionPoolState.Initial);
            _maintenanceCancellationTokenSource = new CancellationTokenSource();
        }
        #endregion

        #region Utilities
        private void ThrowIfDisposed()
        {
            if (_state.Value == ConnectionPoolState.Disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        private void ThrowIfNotOpen()
        {
            if (_state.Value != ConnectionPoolState.Open)
            {
                ThrowIfDisposed();
                throw new InvalidOperationException("ConnectionPool must be initialized.");
            }
        }

        internal PooledConnection CreateNewConnection()
        {
            var connection = _connectionFactory.CreateConnection(_connectionString);
            return new PooledConnection(this, connection);
        }

        internal void ReleaseConnection(PooledConnection connection)
        {
            if (_state.Value == ConnectionPoolState.Disposed)
            {
                connection.Dispose();
                return;
            }

            ListConnectionHolder.Return(connection);
            _poolQueue.Release();
        }

        // 每隔一段时间 重新维护 连接
        private async Task MaintainSizeAsync()
        {
            var maintenanceCancellationToken = _maintenanceCancellationTokenSource.Token;
            while (!maintenanceCancellationToken.IsCancellationRequested)
            {
                try
                {
                    await PrunePoolAsync(maintenanceCancellationToken).ConfigureAwait(false);
                    await EnsureMinSizeAsync(maintenanceCancellationToken).ConfigureAwait(false);
                    await Task.Delay(_options.MaintenanceInterval, maintenanceCancellationToken).ConfigureAwait(false);
                }
                catch { }
            }
        }

        private async Task PrunePoolAsync(CancellationToken cancellationToken)
        {
            bool enteredPool = false;
            try
            {
                enteredPool = await _poolQueue.WaitAsync(TimeSpan.FromMilliseconds(20), cancellationToken).ConfigureAwait(false);
                if (!enteredPool)
                {
                    return;
                }

                ListConnectionHolder.Prune();
            }
            finally
            {
                if (enteredPool)
                {
                    try
                    {
                        _poolQueue.Release();
                    }
                    catch { }
                }
            }
        }

        private async Task EnsureMinSizeAsync(CancellationToken cancellationToken)
        {
            while (CreatedCount < _options.MinConnections)
            {
                bool enteredPool = false;
                try
                {
                    enteredPool = await _poolQueue.WaitAsync(TimeSpan.FromMilliseconds(20), cancellationToken).ConfigureAwait(false);
                    if (!enteredPool)
                    {
                        return;
                    }

                    var connection = CreateNewConnection();
                    connection.Open();
                    ListConnectionHolder.Return(connection);
                }
                finally
                {
                    if (enteredPool)
                    {
                        try
                        {
                            _poolQueue.Release();
                        }
                        catch { }
                    }
                }
            }
        }

        #endregion

        #region Properties

        public string ConnectionString => _connectionString;

        public int Version
        {
            get { return Interlocked.CompareExchange(ref _version, 0, 0); }
        }

        public int CreatedCount
        {
            get
            {
                ThrowIfDisposed();
                return UsedCount + DormantCount;
            }
        }

        public int DormantCount
        {
            get
            {
                ThrowIfDisposed();
                return ListConnectionHolder.Count;
            }
        }

        public int UsedCount
        {
            get
            {
                ThrowIfDisposed();
                return _options.MaxConnections - AvailableCount;
            }
        }

        public int AvailableCount
        {
            get
            {
                ThrowIfDisposed();
                return _poolQueue.CurrentCount;
            }
        }
        #endregion

        #region Methods
        public void Initialize()
        {
            ThrowIfDisposed();
            if (_state.TryChange(ConnectionPoolState.Initial, ConnectionPoolState.Open))
                MaintainSizeAsync().ConfigureAwait(false);
        }

        public DbConnection RentConnection()
        {
            ThrowIfNotOpen();

            var helper = new RentHelper(this);
            try
            {
                helper.CheckingOutConnection();
                var enteredPool = _poolQueue.Wait(_options.WaitQueueTimeout);
                return helper.EnteredPool(enteredPool);
            }
            catch (Exception ex)
            {
                helper.HandleException(ex);
                throw;
            }
            finally
            {
                helper.Finally();
            }
        }

        public void Clear()
        {
            ThrowIfNotOpen();
            Interlocked.Increment(ref _version);
        }

        public void Dispose()
        {
            if (_state.TryChange(ConnectionPoolState.Disposed))
            {
                ListConnectionHolder.Clear();

                _maintenanceCancellationTokenSource.Cancel();
                _maintenanceCancellationTokenSource.Dispose();

                _poolQueue.Dispose();
                WaitQueue.Dispose();
            }
        }

        #endregion
    }
}
