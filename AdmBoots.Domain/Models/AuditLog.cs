using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdmBoots.Infrastructure.Domain;
using AdmBoots.Infrastructure.Framework.Abstractions;

namespace AdmBoots.Domain.Models {

    public class AuditLog : Entity {

        /// <summary>
        /// 用户ID
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// 服务 (类/接口) 名
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength250)]
        public string ServiceName { get; set; }

        /// <summary>
        /// 执行方法名称
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength250)]
        public string MethodName { get; set; }

        /// <summary>
        /// 调用参数
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength2000)]
        public string Parameters { get; set; }

        /// <summary>
        /// 返回值
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength2000)]
        public string ReturnValue { get; set; }

        /// <summary>
        /// 方法执行的开始时间
        /// </summary>
        public DateTime ExecutionTime { get; set; }

        /// <summary>
        /// 方法调用的总持续时间（毫秒）
        /// </summary>
        public int ExecutionDuration { get; set; }

        /// <summary>
        /// 客户端的IP地址
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength50)]
        public string ClientIpAddress { get; set; }

        /// <summary>
        /// 客户端的名称（通常是计算机名）
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength100)]
        public string ClientName { get; set; }

        /// <summary>
        /// 浏览器信息
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength250)]
        public string BrowserInfo { get; set; }

        /// <summary>
        /// 方法执行期间发生异常
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength2000)]
        public string Exception { get; set; }

        /// <summary>
        /// 自定义数据
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength2000)]
        public string CustomData { get; set; }
    }
}
