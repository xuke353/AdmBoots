using System;
using System.Collections.Generic;
using System.Text;

namespace AdmBoots.Infrastructure.Mail {

    public class MailConfig {

        /// <summary>
        /// 客户端授权码(可存在配置文件中)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 收件人地址(多人)
        /// </summary>
        public string[] ToArry { get; set; } = new string[] { };

        /// <summary>
        /// 发件服务器
        /// </summary>

        public string FrHost { get; set; }

        /// <summary>
        /// 发件人
        /// </summary>
        public string Fr { get; set; }

        /// <summary>
        /// 抄送地址(多人)
        /// </summary>
        public string[] CcArray { get; set; } = new string[] { };
    }
}
