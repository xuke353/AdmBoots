using System;
using System.Collections.Generic;
using System.Text;

namespace AdmBoots.Quartz.Dto {
    public class GetScheduleOutput : AddScheduleInput {
        /// <summary>
        /// 下次执行时间
        /// </summary>
        public DateTime? NextFireTime { get; set; }

        /// <summary>
        /// 上次执行时间
        /// </summary>
        public DateTime? PreviousFireTime { get; set; }
    }
}
