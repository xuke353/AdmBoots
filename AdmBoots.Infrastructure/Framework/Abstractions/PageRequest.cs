using System;
using System.Collections.Generic;
using System.Text;

namespace AdmBoots.Infrastructure.Framework.Abstractions {

    /// <summary>
    /// 分页查询请求
    /// </summary>
    public class PageRequest : SortRequest {

        /// <summary>
        /// 默认数量
        /// </summary>
        public static readonly int DefaultPageSize = 20;

        private int _pageSize = DefaultPageSize;

        /// <summary>
        /// 默认初始页
        /// </summary>
        public static readonly int DefaultPageIndex = 1;

        private int _pageIndex = DefaultPageIndex;

        /// <summary>
        /// 每页数量，如小于等于 0 将被设置为默认数量 20
        /// </summary>
        public int PageSize {
            get => _pageSize;
            set => _pageSize = value <= 0 ? DefaultPageSize : value;
        }

        /// <summary>
        /// 第几页，以 0 开始
        /// </summary>
        public int PageIndex {
            get => _pageIndex;
            set => _pageIndex = value <= 0 ? DefaultPageIndex : value;
        }

        public PageRequest() {
        }

        public PageRequest(int pageSize, int pageIndex = 1, string sortField = "", bool isDesc = false) : base(sortField, isDesc) {
            PageSize = pageSize;
            PageIndex = pageIndex;
        }
    }
}
