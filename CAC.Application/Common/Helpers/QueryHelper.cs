using System.Linq.Expressions;
using System.Reflection;

namespace CAC.Application.Common.Helpers;

public static class QueryHelper
{
    public static IQueryable<T> ApplySorting<T>(IQueryable<T> query, string? sortBy, string sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query;

        var propertyInfo = typeof(T).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        
        if (propertyInfo == null)
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyInfo);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = sortDirection.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new Type[] { typeof(T), propertyInfo.PropertyType },
            query.Expression,
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<T>(resultExpression);
    }
}

