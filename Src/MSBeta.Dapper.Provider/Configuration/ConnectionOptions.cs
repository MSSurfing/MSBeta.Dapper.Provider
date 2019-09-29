using System;

namespace DapperExtensions.Configuration
{
    public class ConnectionOptions
    {
        #region Fields & Consts
        // temp & ToImprove
        private const int MAX_CONNECTIONS = 500;
        private const int MIN_CONNECTIONS = 0;
        private const int WAIT_QUEUE_SIZE = 2000;
        #endregion

        /// <param name="maxConnections">线程池最大连接数</param>
        /// <param name="minConnections">最小连接数</param>
        /// <param name="waitQueueSize">等待队列大小</param>
        /// <param name="waitQueueTimeout">等待超时时间（默认2分钟）</param>
        /// <param name="maintenanceInterval">线程池连接维护间隔（将会释放过期的连接）</param>
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
