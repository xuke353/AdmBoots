using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Adm.Boot.Infrastructure.Framework.Abstractions {
    /// <summary>
    /// 排序条件
    /// </summary>
    public class SortRequest {
        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortField { get; set; }

        /// <summary>
        /// 是否为降序
        /// </summary>
        public bool IsDesc { get; set; }

        /// <summary>
        /// 排序语句
        /// </summary>
        [JsonIgnore]
        public string Ordering => string.IsNullOrEmpty(SortField) ? "" : SortField + (IsDesc ? " DESC" : "");

        public SortRequest() {
        }

        public SortRequest(string sortField = "", bool isDesc = false) {
            SortField = sortField;
            IsDesc = isDesc;
        }
    }
}
