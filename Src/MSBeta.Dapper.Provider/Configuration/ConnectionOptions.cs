using System;

namespace DapperExtensions.Configuration
{
    public class ConnectionOptions
    {
        #region Fields & Consts
        // temp & ToImprove
        private const int MAX_CONNECTIONS = 100;
        private const int MIN_CONNECTIONS = 0;
        private const int WAIT_QUEUE_SIZE = 200;
        #endregion

        public ConnectionOptions(int maxConnections = MAX_CONNECTIONS, int minConnections = MIN_CONNECTIONS, int waitQueueSize = WAIT_QUEUE_SIZE,
            TimeSpan? waitQueueTimeout = null, TimeSpan? maintenanceInterval = null)
        {
            MaintenanceInterval = maintenanceInterval ?? TimeSpan.FromMinutes(2);
            MaxConnections = maxConnections;
            MinConnections = minConnections;
            WaitQueueSize = waitQueueSize;
            WaitQueueTimeout = waitQueueTimeout ?? TimeSpan.FromMinutes(2);
        }

        public TimeSpan MaintenanceInterval { get; private set; }

        public int MaxConnections { get; private set; }

        public int MinConnections { get; private set; }

        public int WaitQueueSize { get; private set; }

        public TimeSpan WaitQueueTimeout { get; private set; }
    }
}
