using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DapperExtensions
{
    public static class PredicateGroupExtensions
    {
        public static PredicateGroup NewFilter(GroupOperator groupOperator = GroupOperator.And)
        {
            var where = new PredicateGroup() { Operator = groupOperator, Predicates = new List<IPredicate>() };
            return where;
        }

        public static PredicateGroup Where(this PredicateGroup _this, IPredicate item)
        {
            _this.Predicates.Add(item);
            return _this;
        }

        public static PredicateGroup Where<T>(this PredicateGroup _this, Expression<Func<T, object>> expression, Operator op, object value, bool not = false)
            where T : class
        {
            _this.Predicates.Add(Field(expression, op, value, not));
            return _this;
        }

        public static ISort Sort<T>(this PredicateGroup _this, Expression<Func<T, object>> expression, bool ascending = true)
        {
            return Predicates.Sort<T>(expression, ascending);
        }

        private static IFieldPredicate Field<T>(Expression<Func<T, object>> expression, Operator op, object value, bool not = false)
            where T : class
        {
            return Predicates.Field<T>(expression, op, value, not);
        }

    }
}
