using DapperExtensions.Connections;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace DapperExtensions
{
    public class DapperRepository<T> : IRepository<T> where T : class
    {
        #region Fields
        private readonly DbConnection _dbConnection;
        private DbTransaction _dbTransaction = null;
        #endregion

        #region Ctor
        public DapperRepository() : this(DapperProvider.ConnectionPool.RentConnection()) { }
        public DapperRepository(IConnectionPool _connectionPool) : this(_connectionPool.RentConnection()) { }
        public DapperRepository(IDbContext dbContext) : this(dbContext.GetDbConnection()) { }
        public DapperRepository(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #endregion

        #region Utilities
        protected virtual DbConnection DbConnection => _dbConnection;
        #endregion

        #region Properties
        public virtual DapperFilter<T> FilterAnd => new DapperFilter<T>(GroupOperator.And);
        public virtual DapperFilter<T> FilterOr => new DapperFilter<T>(GroupOperator.Or);

        public virtual int? CommandTimeout { get; set; }
        #endregion

        #region Get / Count / Exists
        // 不推荐
        //IEnumerable<T> GetFromSql(string sql, object param = null){}

        public virtual IEnumerable<T> GetPaged(IPredicate where = null, ISort sort = null, int pageIndex = 0, int pageSize = 30)
        {
            ISort[] sorts = null;
            if (sort != null)
                sorts = new[] { sort };

            return _dbConnection.GetPage<T>(where, sorts, pageIndex, pageSize, _dbTransaction, CommandTimeout);
        }

        public virtual IEnumerable<T> GetPaged(IPredicate where = null, ISort[] sort = null, int pageIndex = 0, int pageSize = 30)
        {
            return _dbConnection.GetPage<T>(where, sort, pageIndex, pageSize, _dbTransaction, CommandTimeout);
        }

        public virtual IEnumerable<T> GetList(IPredicate where = null, params ISort[] sort)
        {
            return _dbConnection.GetList<T>(where, sort, _dbTransaction, CommandTimeout);
        }

        public virtual T GetOne(IPredicate where = null, params ISort[] sort)
        {
            return _dbConnection.GetList<T>(where, sort, _dbTransaction, CommandTimeout).FirstOrDefault();
        }

        public virtual T GetById(object Id)
        {
            return _dbConnection.Get<T>(Id, _dbTransaction, CommandTimeout);
        }

        public virtual int Count(IPredicate where)
        {
            return _dbConnection.Count<T>(where, _dbTransaction, CommandTimeout);
        }

        public virtual bool Exists(IPredicate where)
        {
            // ToImprove, Use ExistsPredicate
            return _dbConnection.Count<T>(where, _dbTransaction, CommandTimeout) > 0;
        }
        #endregion

        #region Insert / Update / Delete

        public virtual bool Insert(T entity)
        {
            var result = this.InsertDynamic(entity);
            if (result == null)
                return false;

            return true;
        }

        public virtual bool Insert(T entity, out string Id)
        {
            Id = null;
            var result = this.InsertDynamic(entity);
            if (result == null)
                return false;

            Id = result.ToString();
            return true;
        }

        public virtual dynamic InsertDynamic(T entity)
        {
            if (entity == null)
                return null;

            return _dbConnection.Insert<T>(entity, _dbTransaction, CommandTimeout);
        }

        public virtual void Insert(IEnumerable<T> entities)
        {
            _dbConnection.Insert<T>(entities, _dbTransaction, CommandTimeout);
        }

        public virtual bool Update(T entity)
        {
            return _dbConnection.Update(entity, _dbTransaction, CommandTimeout);
        }

        public virtual bool Delete(T entity)
        {
            return _dbConnection.Delete<T>(entity, _dbTransaction, CommandTimeout);
        }

        public virtual bool Delete(IPredicate where)
        {
            return _dbConnection.Delete<T>(where, _dbTransaction, CommandTimeout);
        }
        #endregion

        #region Use DbTransaction
        public virtual IDisposable UseTransaction(DbTransaction transaction)
        {
            return new Transactor(this, transaction); ;
        }

        public void Dispose()
        {
            _dbConnection.Dispose();
            _dbTransaction = null;
        }
        #endregion

        #region Nested class for using transaction
        private sealed class Transactor : IDisposable
        {
            private readonly DapperRepository<T> _repository;

            public Transactor(DapperRepository<T> repository, DbTransaction dbTransaction)
            {
                _repository = repository;
                _repository._dbTransaction = dbTransaction;
            }

            public void Dispose() => _repository._dbTransaction = null;
        }
        #endregion
    }
}
