using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using AdmBoots.Infrastructure.Domain;
using AdmBoots.Infrastructure.Framework.Abstractions;

namespace AdmBoots.Domain.Models {

    [Table("JobLog")]
    public class JobLog : Entity<int> {

        /// <summary>
        /// 开始执行时间
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 任务名称 组名.任务名
        /// </summary>
        [Required, MaxLength(EntityDefault.FieldsLength50)]
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
