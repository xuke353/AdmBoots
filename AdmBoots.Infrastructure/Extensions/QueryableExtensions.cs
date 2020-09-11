using System;
using System.Linq;
using System.Linq.Expressions;
using AdmBoots.Infrastructure.Framework.Abstractions;
using System.Linq.Dynamic.Core;

namespace AdmBoots.Infrastructure.Extensions {

    public static class QueryableExtensions {

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="TSource">查询对象的类型</typeparam>
        /// <param name="source">原始查询</param>
        /// <param name="sortRequest">排序请求</param>
        /// <returns>排序后的查询</returns>
        public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, SortRequest sortRequest) {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }
            if (sortRequest == null) {
                throw new ArgumentNullException(nameof(sortRequest));
            }

            var result = source;
            if (!string.IsNullOrEmpty(sortRequest.Ordering)) {
                result = source.OrderBy(sortRequest.Ordering);
            }

            return result;
        }

        /// <summary>
        /// 排序后并分页
        /// </summary>
        /// <typeparam name="TSource">查询对象的类型</typeparam>
        /// <param name="source">原始查询</param>
        /// <param name="pageRequest">分页请求</param>
        /// <returns>分页后的查询</returns>
        public static IQueryable<TSource> PageAndOrderBy<TSource>(this IQueryable<TSource> source, PageRequest pageRequest) {
            return source.OrderBy(pageRequest).PageBy(pageRequest);
        }

        /// <summary>
        /// 使用默认排序进行分页
        /// </summary>
        /// <typeparam name="TSource">查询对象的类型</typeparam>
        /// <param name="source">原始查询</param>
        /// <param name="pageRequest">分页请求</param>
        /// <returns>分页后的查询</returns>
        public static IQueryable<TSource> PageBy<TSource>(this IQueryable<TSource> source, PageRequest pageRequest) {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }
            if (pageRequest == null) {
                throw new ArgumentNullException(nameof(pageRequest));
            }
            if (pageRequest.PageIndex < 0) {
                throw new ArgumentOutOfRangeException(nameof(pageRequest.PageIndex));
            }

            var result = source;
            var pageIndex = pageRequest.PageIndex;
            var pageSize = pageRequest.PageSize;
            // 为了优化 dotConnect for Oracle 生成的分页查询 SQL，针对首页不做 Skip(0)
            if (pageIndex > 0) {
                result = result.Skip((pageIndex - 1) * pageSize);
            }
            return result.Take(pageSize);
        }

        /// <summary>
        /// 如果条件为 true，则为 <see cref="IQueryable{T}"/> 增加过滤条件
        /// </summary>
        /// <param name="query">原始查询</param>
        /// <param name="condition">条件</param>
        /// <param name="predicate">过滤条件</param>
        /// <returns>可能应用了过滤条件的查询</returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate) {
            return condition
                ? query.Where(predicate)
                : query;
        }

        /// <summary>
        /// 如果条件为 true，则为 <see cref="IQueryable{T}"/> 增加过滤条件
        /// </summary>
        /// <param name="query">原始查询</param>
        /// <param name="condition">条件</param>
        /// <param name="predicate">过滤条件</param>
        /// <returns>可能应用了过滤条件的查询</returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, int, bool>> predicate) {
            return condition
                ? query.Where(predicate)
                : query;
        }
    }
}
