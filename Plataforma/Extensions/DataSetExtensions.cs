using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Plataforma.Data.Interceptors;

namespace Plataforma.Extensions;

public static class DataSetExtensions {
    private static TResult WithNoLock<T, TResult>(this IQueryable<T> query, Func<IQueryable<T>, TResult> queryAction) {
        var oldValue = WithNoLockInterceptor.WithNoLock;
        WithNoLockInterceptor.WithNoLock = true;
        var queryableResult = queryAction(query);
        WithNoLockInterceptor.WithNoLock = oldValue;
        return queryableResult;
    }

    public static Task<List<T>> WithNoLockToListAsync<T>(this IQueryable<T> query) {
        return query.WithNoLock(q => q.ToListAsync());
    }
    public static List<T> WithNoLockToList<T>(this IQueryable<T> query) {
        return query.WithNoLock(q => q.ToList());
    }
    public static Task<T> WithNoLockFirstOrDefaultAsync<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate) {
        return query.WithNoLock(q => q.FirstOrDefaultAsync(predicate));
    }
    public static Task<int> WithNoLockCountAsync<T>(this IQueryable<T> query) {
        return query.WithNoLock(q => q.CountAsync());
    }
    public static T WithNoLockFirstOrDefault<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate) {
        return query.WithNoLock(q => q.FirstOrDefault(predicate));
    }
    public static Task<T> WithNoLockFirstOrDefaultAsync<T>(this IQueryable<T> query) {
        return query.WithNoLock(q => q.FirstOrDefaultAsync());
    }
    public static T WithNoLockFirstOrDefault<T>(this IQueryable<T> query) {
        return query.WithNoLock(q => q.FirstOrDefault());
    }
    public static int WithNoLockCount<T>(this IQueryable<T> query) {
        return query.WithNoLock(q => q.Count());
    }
}