using DapperExtensions.Connections.Internal;
using DapperExtensions.Exceptions;
using DapperExtensions.Internal;
using System;
using System.Diagnostics;

namespace DapperExtensions.Connections
{
    internal class RentHelper
    {
        private readonly ConnectionPool _pool;
        private Stopwatch _stopwatch;

        private bool _enteredPool;
        private bool _enteredWaitQueue;

        public RentHelper(ConnectionPool pool)
        {
            _pool = pool;
        }

        #region Utilities
        private RentedConnection RentOrCreateConnection()
        {
            PooledConnection connection = _pool.ListConnectionHolder.Rent();
            if (connection == null)
            {
                connection = _pool.CreateNewConnection();
            }

            var reference = new ReferenceCounted<PooledConnection>(connection, _pool.ReleaseConnection);
            return new RentedConnection(_pool, reference);
        }
        #endregion

        #region Methods
        public void CheckingOutConnection()
        {
            _enteredWaitQueue = _pool.WaitQueue.Wait(0);
            if (!_enteredWaitQueue)
                throw new WaitQueueFullException();

            _stopwatch = Stopwatch.StartNew();
        }

        public RentedConnection EnteredPool(bool enteredPool)
        {
            _enteredPool = enteredPool;
            if (enteredPool)
            {
                var acquired = RentOrCreateConnection();
                _stopwatch.Stop();

                return acquired;
            }

            _stopwatch.Stop();
            var message = string.Format("Timed out waiting for a connection after {0}ms.", _stopwatch.ElapsedMilliseconds);
            throw new TimeoutException(message);
        }

        public void Finally()
        {
            if (_enteredWaitQueue)
            {
                try
                {
                    _pool.WaitQueue.Release();
                }
                catch { }
            }
        }

        public void HandleException(Exception ex)
        {
            if (_enteredPool)
            {
                try
                {
                    _pool.WaitQueue.Release();
                }
                catch { }
            }
        }
        #endregion
    }
}
