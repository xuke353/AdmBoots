using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdmBoots.Quartz.Dto {
    public class GetScheduleOutput : AddScheduleInput {
        public string Id => $"{GroupName}-{JobName}";
        /// <summary>
        /// 下次执行时间
        /// </summary>
        public DateTime? NextFireTime { get; set; }

        /// <summary>
        /// 上次执行时间
        /// </summary>
        public DateTime? PreviousFireTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter), ItemConverterParameters = new object[] { true })]
        public IReadOnlyCollection<JobOption> Options => Status switch
        {
            0 => new JobOption[] {
                            JobOption.pause,
                            JobOption.execute,
                        },
            1 => new JobOption[] {
                            JobOption.resume,
                        },
            _ => new JobOption[] {
                            JobOption.execute,
                            JobOption.resume,
                            JobOption.pause,
                            JobOption.stop,
                        },
            // _ => Array.Empty<JobOption>(),
        };
    }

    public enum JobOption {
        execute,
        stop,//先不用
        resume,
        pause,
    }
}
