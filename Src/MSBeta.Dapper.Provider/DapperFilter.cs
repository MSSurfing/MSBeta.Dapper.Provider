using DapperExtensions;
using DapperExtensions.Sql;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DapperExtensions
{
    public class DapperFilter<T> : IPredicateGroup where T : class
    {
        #region Fields & Ctor
        private readonly PredicateGroup filter;

        public DapperFilter(GroupOperator groupOperator = GroupOperator.And)
        {
            filter = PredicateGroupExtensions.NewFilter(groupOperator);
        }
        #endregion

        #region ex Methods
        public virtual DapperFilter<T> Where(IPredicate item)
        {
            PredicateGroupExtensions.Where(filter, item);
            return this;
        }

        public virtual DapperFilter<T> Where(Expression<Func<T, object>> expression, Operator op, object value, bool not = false)
        {
            PredicateGroupExtensions.Where<T>(filter, expression, op, value, not);
            return this;
        }

        public virtual ISort Sort(Expression<Func<T, object>> expression, bool ascending = true)
        {
            return PredicateGroupExtensions.Sort<T>(filter, expression, ascending);
        }
        #endregion

        #region IPredicateGroup Methods
        public string GetSql(ISqlGenerator sqlGenerator, IDictionary<string, object> parameters)
        {
            return filter.GetSql(sqlGenerator, parameters);
        }

        public GroupOperator Operator { get => filter.Operator; set => filter.Operator = value; }
        public IList<IPredicate> Predicates { get => filter.Predicates; set => filter.Predicates = value; }
        #endregion
    }
}
