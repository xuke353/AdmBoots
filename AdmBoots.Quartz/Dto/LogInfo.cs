using System;
using System.Collections.Generic;
using System.Text;

namespace AdmBoots.Quartz.Dto {
    public class LogInfo {
        /// <summary>
        /// 开始执行时间
        /// </summary>
        public string BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// 耗时（秒）
        /// </summary>
        public double Seconds { get; set; }
        /// <summary>
        /// 错误级别
        /// </summary>
        public string Level { get; set; }
        /// <summary>
        /// 请求结果
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 异常消息
        /// </summary>
        public string ErrorMsg { get; set; }
    }
}
