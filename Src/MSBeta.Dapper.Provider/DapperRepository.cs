using DapperExtensions.Connections;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DapperExtensions
{
    public class DapperRepository<T> : IRepository<T> where T : class
    {
        #region Fields
        private readonly IDbConnection _dbConnection;
        private IDbTransaction _dbTransaction = null;
        #endregion

        #region Ctor
        public DapperRepository() : this(DapperProvider.ConnectionPool.RentConnection()) { }
        public DapperRepository(IConnectionPool _connectionPool) : this(_connectionPool.RentConnection()) { }
        public DapperRepository(IDbContext dbContext) : this(dbContext.GetDbConnection()) { }
        public DapperRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #endregion

        #region Utilities
        protected virtual IDbConnection DbConnection => _dbConnection;
        #endregion

        #region Properties

        public virtual DapperFilter<T> FilterAnd => new DapperFilter<T>(GroupOperator.And);
        public virtual DapperFilter<T> FilterOr => new DapperFilter<T>(GroupOperator.Or);

        public virtual int? CommandTimeout { get; set; }

        #endregion

        #region Get / Count / Exists

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

        #region Async Get / Count / Exists

        public virtual async Task<IEnumerable<T>> GetPagedAsync(IPredicate where = null, ISort sort = null, int pageIndex = 0, int pageSize = 30)
        {
            ISort[] sorts = null;
            if (sort != null)
                sorts = new[] { sort };

            return await _dbConnection.GetPageAsync<T>(where, sorts, pageIndex, pageSize, _dbTransaction, CommandTimeout);
        }

        public virtual async Task<IEnumerable<T>> GetPagedAsync(IPredicate where = null, ISort[] sort = null, int pageIndex = 0, int pageSize = 30)
        {
            return await _dbConnection.GetPageAsync<T>(where, sort, pageIndex, pageSize, _dbTransaction, CommandTimeout);
        }

        public virtual async Task<IEnumerable<T>> GetListAsync(IPredicate where = null, params ISort[] sort)
        {
            return await _dbConnection.GetListAsync<T>(where, sort, _dbTransaction, CommandTimeout);
        }

        public virtual async Task<T> GetOneAsync(IPredicate where = null, params ISort[] sort)
        {
            return (await _dbConnection.GetListAsync<T>(where, sort, _dbTransaction, CommandTimeout)).FirstOrDefault();
        }

        public virtual async Task<T> GetByIdAsync(object Id)
        {
            return await _dbConnection.GetAsync<T>(Id, _dbTransaction, CommandTimeout);
        }

        public virtual async Task<int> CountAsync(IPredicate where)
        {
            return await _dbConnection.CountAsync<T>(where, _dbTransaction, CommandTimeout);
        }

        public virtual async Task<bool> ExistsAsync(IPredicate where)
        {
            // ToImprove, Use ExistsPredicate
            return await _dbConnection.CountAsync<T>(where, _dbTransaction, CommandTimeout) > 0;
        }
        #endregion

        #region Async Insert / Update / Delete


        public virtual async Task<string> InsertAsync(T entity)
        {
            var result = await this.InsertDynamicAsync(entity);
            if (result == null)
                return string.Empty;

            return result.ToString();
        }

        public virtual async Task<dynamic> InsertDynamicAsync(T entity)
        {
            if (entity == null)
                return null;

            return await _dbConnection.InsertAsync<T>(entity, _dbTransaction, CommandTimeout);
        }

        public virtual async Task InsertAsync(IEnumerable<T> entities)
        {
            await _dbConnection.InsertAsync<T>(entities, _dbTransaction, CommandTimeout);
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            return await _dbConnection.UpdateAsync(entity, _dbTransaction, CommandTimeout);
        }

        public virtual async Task<bool> DeleteAsync(T entity)
        {
            return await _dbConnection.DeleteAsync<T>(entity, _dbTransaction, CommandTimeout);
        }

        public virtual async Task<bool> DeleteAsync(IPredicate where)
        {
            return await _dbConnection.DeleteAsync<T>(where, _dbTransaction, CommandTimeout);
        }
        #endregion

        #region Use IDbTransaction
        public virtual IDisposable UseTransaction(IDbTransaction transaction)
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

            public Transactor(DapperRepository<T> repository, IDbTransaction IDbTransaction)
            {
                _repository = repository;
                _repository._dbTransaction = IDbTransaction;
            }

            public void Dispose() => _repository._dbTransaction = null;
        }
        #endregion
    }
}
