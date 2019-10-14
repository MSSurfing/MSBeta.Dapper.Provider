using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DapperExtensions
{
    public interface IRepository<T> : IDisposable where T : class
    {
        #region Properties
        DapperFilter<T> FilterAnd { get; }
        DapperFilter<T> FilterOr { get; }

        int? CommandTimeout { get; set; }
        #endregion


        #region Sync

        #region Get / Count / Exists

        IEnumerable<T> GetPaged(IPredicate where = null, ISort sort = null, int pageIndex = 0, int pageSize = 30);

        IEnumerable<T> GetPaged(IPredicate where = null, ISort[] sort = null, int pageIndex = 0, int pageSize = 30);

        IEnumerable<T> GetList(IPredicate where = null, params ISort[] sort);

        T GetOne(IPredicate where = null, params ISort[] sort);

        T GetById(object Id);

        int Count(IPredicate where);

        bool Exists(IPredicate where);

        #endregion

        #region Insert / Update / Delete
        /// <summary>
        /// Dapper.Extensions.Insert 方法 返回值是 表的主键值，而不是影响行数或bool。
        ///     如果 表主键是 Guid 则返回 36位字符串
        ///     如果 表主键是 Int 则返回 数字字符串
        /// 授 Dapper.Extensions.Insert 影响 不得不返回 string
        /// </summary>
        bool Insert(T entity);
        bool Insert(T entity, out string Id);

        dynamic InsertDynamic(T entity);

        void Insert(IEnumerable<T> entities);

        bool Update(T entity);

        bool Delete(T entity);

        bool Delete(IPredicate where);
        #endregion

        #endregion


        #region Async

        #region Get / Count / Exists
        Task<IEnumerable<T>> GetPagedAsync(IPredicate where = null, ISort sort = null, int pageIndex = 0, int pageSize = 30);

        Task<IEnumerable<T>> GetPagedAsync(IPredicate where = null, ISort[] sort = null, int pageIndex = 0, int pageSize = 30);

        Task<IEnumerable<T>> GetListAsync(IPredicate where = null, params ISort[] sort);

        Task<T> GetOneAsync(IPredicate where = null, params ISort[] sort);

        Task<T> GetByIdAsync(object Id);

        Task<int> CountAsync(IPredicate where);

        Task<bool> ExistsAsync(IPredicate where);

        #endregion

        #region Insert / Update / Delete

        /// <summary>
        /// Dapper.Extensions.Insert 方法 返回值是 表的主键值，而不是影响行数或bool。
        ///     如果 表主键是 Guid 则返回 36位字符串
        ///     如果 表主键是 Int 则返回 数字字符串
        /// 授 Dapper.Extensions.Insert 影响 不得不返回 string
        /// </summary>
        Task<string> InsertAsync(T entity);

        Task<dynamic> InsertDynamicAsync(T entity);

        Task InsertAsync(IEnumerable<T> entities);

        Task<bool> UpdateAsync(T entity);

        Task<bool> DeleteAsync(T entity);

        Task<bool> DeleteAsync(IPredicate where);

        #endregion

        #endregion

        #region Use IDbTransaction
        IDisposable UseTransaction(IDbTransaction transaction);
        #endregion
    }
}
