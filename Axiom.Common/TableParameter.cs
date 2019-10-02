using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace AXIOM.Common
{

    public enum Operations
    {
        /// <summary>
        /// The starts with
        /// </summary>
        StartsWith = 1,

        /// <summary>
        /// The contains
        /// </summary>
        Contains = 2,

        /// <summary>
        /// The equals
        /// </summary>
        Equals,
    }
    /// <summary>
    /// Expression Builder
    /// </summary>
    public static class ExpressionBuilder
    {
        /// <summary>
        /// The contains method
        /// </summary>
        private static MethodInfo containsMethod = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });

        /// <summary>
        /// The starts with method
        /// </summary>
        private static MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });

        /// <summary>
        /// The equals method
        /// </summary>
        private static MethodInfo equalsMethod = typeof(string).GetMethod("Equals", new Type[] { typeof(string) });

        /// <summary>
        /// Dynamics the order by.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="orderByProperty">The order by property.</param>
        /// <param name="desc">if set to <c>true</c> [descending].</param>
        /// <returns>TEntity</returns>
        public static IQueryable<TEntity> DynamicOrderBy<TEntity>(this IQueryable<TEntity> source, string orderByProperty,
                          bool desc)
        {
            string command = desc ? "OrderByDescending" : "OrderBy";
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }

        /// <summary>
        /// Dynamics the where.
        /// </summary>
        /// <typeparam name="T">type of class</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>T</returns>
        public static IQueryable<T> DynamicWhere<T>(this IQueryable<T> query, TableParameter<T> filter) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "type");
            var propertyExpression = Expression.Property(parameter, filter.SearchKey);
            ConstantExpression searchValue = null;

            PropertyInfo p = typeof(T).GetProperty(filter.SearchKey);
            Type t = p.PropertyType;

            if (t == typeof(Nullable<int>))
            {
                searchValue = Expression.Constant(Convert.ToInt32(filter.SearchValue), typeof(Nullable<int>));
            }
            else
            {
                searchValue = Expression.Constant(filter.SearchValue, typeof(string));
            }

            Expression expression = null;

            switch (filter.Operation)
            {
                case Operations.Equals:
                    expression = Expression.Call(propertyExpression, equalsMethod, searchValue);
                    break;
                case Operations.Contains:
                    expression = Expression.Call(propertyExpression, containsMethod, searchValue);
                    break;
                case Operations.StartsWith:
                    expression = Expression.Call(propertyExpression, startsWithMethod, searchValue);
                    break;
                default:
                    return null;
            }

            return query.Where(Expression.Lambda<Func<T, bool>>(expression, parameter));
        }
    }

    /// <summary>
    /// Table Parameter
    /// </summary>
    /// <typeparam name="T">type of class</typeparam>
    public class TableParameter<T> where T : class
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public List<T> aaData { get; set; }

        /// <summary>
        /// Gets or sets the i display start.
        /// </summary>
        /// <value>
        /// The i display start.
        /// </value>
        public int iDisplayStart { get; set; }

        /// <summary>
        /// Gets or sets the display length of the i.
        /// </summary>
        /// <value>
        /// The display length of the i.
        /// </value>
        public int iDisplayLength { get; set; }

        /// <summary>
        /// Gets or sets the i total records.
        /// </summary>
        /// <value>
        /// The i total records.
        /// </value>
        public int iTotalRecords { get; set; }

        /// <summary>
        /// Gets or sets the i total display records.
        /// </summary>
        /// <value>
        /// The i total display records.
        /// </value>
        public int iTotalDisplayRecords { get; set; }

        /// <summary>
        /// Gets or sets the search key.
        /// </summary>
        /// <value>
        /// The search key.
        /// </value>
        public string SearchKey { get; set; }

        /// <summary>
        /// Gets or sets the search value.
        /// </summary>
        /// <value>
        /// The search value.
        /// </value>
        public string SearchValue { get; set; }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public Operations Operation { get; set; }

        /// <summary>
        /// Gets or sets the sort columns.
        /// </summary>
        /// <value>
        /// The sort columns.
        /// </value>
        public string SortColumns { get; set; }

        /// <summary>
        /// Gets the sort column.
        /// </summary>
        /// <value>
        /// The sort column.
        /// </value>
        public SortColumn SortColumn
        {
            get
            {
                if (!string.IsNullOrEmpty(SortColumns))
                {
                    return (SortColumn)JsonConvert.DeserializeObject(SortColumns, typeof(SortColumn));
                }

                return null;
            }
        }

        public int PageIndex
        {
            get;
            set;
        }
        public int page { get; set; }
    }

    /// <summary>
    /// Sort Column
    /// </summary>
    public class SortColumn
    {
        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value>
        /// The column.
        /// </value>
        public string Column { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [descending].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [descending]; otherwise, <c>false</c>.
        /// </value>
        public bool Desc { get; set; }
    }
}
