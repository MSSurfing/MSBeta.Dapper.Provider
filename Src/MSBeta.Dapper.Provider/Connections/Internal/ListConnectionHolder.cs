using System.Collections.Generic;

namespace DapperExtensions.Connections.Internal
{
    internal class ListConnectionHolder
    {
        private readonly object _lock = new object();
        private readonly List<PooledConnection> _connections;

        public ListConnectionHolder()
        {
            _connections = new List<PooledConnection>();
        }

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _connections.Count;
                }
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                foreach (var connection in _connections)
                {
                    RemoveConnection(connection);
                }
                _connections.Clear();
            }
        }

        public void Prune()
        {
            lock (_lock)
            {
                for (int i = 0; i < _connections.Count; i++)
                {
                    if (_connections[i].IsExpired)
                    {
                        RemoveConnection(_connections[i]);
                        _connections.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public PooledConnection Rent()
        {
            lock (_lock)
            {
                if (_connections.Count > 0)
                {
                    var connection = _connections[_connections.Count - 1];
                    _connections.RemoveAt(_connections.Count - 1);
                    if (connection.IsExpired)
                    {
                        RemoveConnection(connection);
                    }
                    else
                    {
                        return connection;
                    }
                }
            }
            return null;
        }

        public void Return(PooledConnection connection)
        {
            // is expired
            if (connection.IsExpired)
            {
                RemoveConnection(connection);
                return;
            }

            lock (_lock)
            {
                _connections.Add(connection);
            }
        }

        private void RemoveConnection(PooledConnection connection)
        {
            connection.Dispose();
        }
    }
}
