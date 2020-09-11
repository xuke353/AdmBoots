using System;
using System.Collections.Generic;
using System.Text;

namespace AdmBoots.Infrastructure.Framework.Abstractions {

    /// <summary>
    /// 分页查询结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Page<T> where T : class {

        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// 当前为第几页，以 1 开始
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public long TotalElements { get; private set; }

        /// <summary>
        /// 当前页内容
        /// </summary>
        public IList<T> Content;

        /// <summary>
        /// 获取总页数
        /// </summary>
        /// <param name="totalCount">总记录数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>总页数，如总记录数或每页数量为非正数，返回 0</returns>
        private static int GetTotalPages(long totalCount, int pageSize) {
            if (totalCount <= 0 || pageSize <= 0) {
                return 0;
            }

            return (int)Math.Ceiling((double)totalCount / pageSize);
        }

        public Page(int pageSize, int pageIndex, long totalCount, IList<T> content) {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalElements = totalCount;
            TotalPages = GetTotalPages(TotalElements, PageSize);
            Content = content;
        }

        public Page(PageRequest request, long totalCount, IList<T> content)
            : this(request.PageSize, request.PageIndex, totalCount, content) {
        }

        /// <summary>
        /// 空结果
        /// </summary>
        /// <returns></returns>
        public static Page<T> Empty() {
            return new Page<T>(0, 1, 0, new List<T>());
        }
    }
}
