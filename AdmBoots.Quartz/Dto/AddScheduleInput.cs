using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Quartz.Common;

namespace AdmBoots.Quartz.Dto {
    public class AddScheduleInput {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }
        /// <summary>
        /// 任务组名
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// Cron表达式
        /// </summary>
        public string Cron { get; set; }
        /// <summary>
        /// 请求连接
        /// </summary>
        public string RequestUrl { get; set; }
        /// <summary>
        /// 请求参数
        /// </summary>
        public string RequestParams { get; set; }
        /// <summary>
        /// Headers(可以包含如：Authorization授权认证)
        /// 格式：{"Authorization":"userpassword.."}
        /// </summary>
        public string Headers { get; set; }
        /// <summary>
        /// 描述信息
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        /// 请求类型
        /// </summary>
        public RequestType RequestType { get; set; } = RequestType.Post;
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
    }
}
