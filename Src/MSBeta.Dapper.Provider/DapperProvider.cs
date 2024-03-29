﻿using DapperExtensions.Configuration;
using DapperExtensions.Connections;
using DapperExtensions.Sql;
using System;

namespace DapperExtensions
{
    public static class DapperProvider
    {
        #region Utilities

        private static ISqlDialect GetSqlDialect(DbOption dbOption)
        {
            ISqlDialect sqlDialect;
            switch (dbOption)
            {
                case DbOption.UseMySql:
                    sqlDialect = new MySqlDialect();
                    break;
                //case DbOptions.UseSqlite:
                //    sqlDialect = new SqliteDialect();
                //    break;
                //case DbOptions.UseSqlCe:
                //    sqlDialect = new SqlCeDialect();
                //    break;
                //case DbOptions.UsePostgreSql:
                //    sqlDialect = new PostgreSqlDialect();
                //    break;
                case DbOption.UseSqlServer:
                default:
                    sqlDialect = new SqlServerDialect();
                    break;
            }
            return sqlDialect;
        }

        private static ConnectionOptions ToOption(int maxConnections, int minConnections, int waitQueueSize,
            TimeSpan? waitQueueTimeout = null, TimeSpan? maintenanceInterval = null)
        {
            return new ConnectionOptions(maxConnections: maxConnections, minConnections: minConnections
                , waitQueueSize: waitQueueSize, waitQueueTimeout: waitQueueTimeout
                , maintenanceInterval: maintenanceInterval);
        }

        #endregion

        #region Properties
        public static DbOption DbOption { get; private set; }

        public static IConnectionPool ConnectionPool { get; private set; }
        #endregion

        #region Initialize methods
        public static IConnectionPool Initialize(string connectionString, DbOption dbOption = DbOption.UseSqlServer)
        {
            return Initialize(connectionString, new ConnectionOptions(), new DefaultConnectionFactory(dbOption));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="maxConnections">线程池最大连接数</param>
        /// <param name="minConnections">最小连接数</param>
        /// <param name="waitQueueSize">等待队列大小</param>
        /// <param name="waitQueueTimeout">等待超时时间（默认2分钟）</param>
        /// <param name="maintenanceInterval">线程池连接维护间隔（将会释放过期的连接）</param>
        /// <param name="dbOption">数据类型</param>
        /// <returns></returns>
        public static IConnectionPool Initialize(string connectionString,
            int maxConnections, int minConnections, int waitQueueSize,
            TimeSpan? waitQueueTimeout = null, TimeSpan? maintenanceInterval = null, DbOption dbOption = DbOption.UseSqlServer)
        {
            return Initialize(connectionString
                , ToOption(maxConnections, minConnections, waitQueueSize, waitQueueTimeout, maintenanceInterval)
                , new DefaultConnectionFactory(dbOption));
        }

        public static IConnectionPool Initialize(string connectionString, ConnectionOptions connectionOption, DbOption dbOption = DbOption.UseSqlServer)
        {
            return Initialize(connectionString, connectionOption, new DefaultConnectionFactory(dbOption));
        }

        public static IConnectionPool Initialize(string connectionString, ConnectionOptions connectionOption, IConnectionFactory connectionFactory)
        {
            Initialize(connectionFactory.DbOption);

            var pool = new ConnectionPool(connectionString, connectionOption, connectionFactory);
            pool.Initialize();

            ConnectionPool = pool;
            return pool;
        }

        public static void Initialize(DbOption dbOption = DbOption.UseSqlServer)
        {
            DbOption = dbOption;
            DapperExtensions.SqlDialect = GetSqlDialect(dbOption);

            ConnectionPool = new EmptyConnectionPool();
        }
        #endregion
    }
}