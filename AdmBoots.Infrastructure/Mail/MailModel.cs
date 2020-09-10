using System;
using System.Collections.Generic;
using System.Text;

namespace AdmBoots.Infrastructure.Mail {

    public class MailModel {

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 正文
        /// </summary>
        public string Body { get; set; }
    }
}
