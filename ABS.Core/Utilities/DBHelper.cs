using ABS.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace ABS.Core
{
    public static class DBHelper
    {
        public static IQueryable<T> DataSort<T>(IQueryable<T> source, string fieldName, string sortDirection)
        {
            #region 升序 or 降序
            var sortType = "ASC";
            string sortMethod = "OrderBy";
            sortDirection = sortDirection != null ? sortDirection : "OrderBy";
            if (string.Equals(sortDirection.Trim(), "asc", StringComparison.OrdinalIgnoreCase))
            {
                sortMethod = "OrderBy";
            }

            if (string.Equals(sortDirection.Trim(), "desc", StringComparison.OrdinalIgnoreCase))
            {
                sortMethod = "OrderByDescending";
                sortType = "DESC";
            }
            #endregion

            #region 匹配属性           
            var orderField = string.Empty;
            Type t = typeof(T);
            var attributes = t.Attributes;
            var fields = t.GetFields();
            var properties = t.GetProperties();
            Type[] types = new Type[2];  //  参数：对象类型，属性类型
            types[0] = typeof(T);
            var isMatch = false;
            foreach (var p in properties)
            {
                isMatch = string.Equals(fieldName, p.Name, StringComparison.OrdinalIgnoreCase);
                if (isMatch)
                {
                    orderField = p.Name;
                    types[1] = p.PropertyType;
                    break;
                }
            }
            if (!isMatch)
            {
                types[1] = properties.First().PropertyType;
                orderField = properties.First().Name;
            }
            #endregion

            #region 构建 表达式树
            PropertyInfo pi = typeof(T).GetProperty(orderField);
            ParameterExpression param = Expression.Parameter(typeof(T), orderField);
            //构建 表达式树，可做复杂查询处理
            Expression expr = Expression.Call(typeof(Queryable), sortMethod, types,
                                             source.Expression,
                                             Expression.Lambda(Expression.Property(param, orderField),
                                             param));
            IQueryable<T> query = source.AsQueryable().Provider.CreateQuery<T>(expr);
            #endregion

            #region  简化版
            var orderExpression = string.Format("{0} {1}", orderField, sortType);
            IQueryable<T> query2 = source.AsQueryable().OrderBy(orderExpression).AsQueryable();
            #endregion

            return query;
        }

        public static IQueryable<T> DataPaging<T>(IQueryable<T> source, int pageNumber, int pageSize)
        {
            if (pageSize == 0)
                pageSize = 10;
            return source.Skip((Math.Abs(pageNumber) - 1) * pageSize).Take(pageSize);
        }

        public static IQueryable<T> JoinFilter<T>(IQueryable<T> source, List<Filter> filters)
        {
            filters = filters.Where(o => o.Value != null).ToList();

            Type t = typeof(T);
            var attributes = t.Attributes;
            var fields = t.GetFields();
            var properties = t.GetProperties();
 

            return source;
        }

        public static IQueryable<T> Sorting<T>(IQueryable<T> source, string fieldName, string sortDirection, int pageIndex, int pageSize)
        {
            IQueryable<T> query = DataSort<T>(source, fieldName, sortDirection);
            return DataPaging(query, pageIndex, pageSize);
        }
    }
 
}
