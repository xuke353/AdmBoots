using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Domain.Models;
using AutoMapper;

namespace AdmBoots.Application.MailSettings.Dto {
    [AutoMap(typeof(MailSetting), ReverseMap = true)]
    public class AddMailSettingInput {
        /// <summary>
        /// 客户端授权码(可存在配置文件中)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 收件人地址(多人)
        /// </summary>
        public string To { get; set; }

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
        public string Cc { get; set; }

        /// <summary>
        /// 是否开启邮件提醒
        /// </summary>
        public bool Notify { get; set; }
    }
}
