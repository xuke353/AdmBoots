using System;
using System.Collections.Generic;
using System.Text;

namespace AdmBoots.Application.Users.Dto {
    public class ModifyPasswordInput {
        /// <summary>
        /// 当前密码
        /// </summary>
        public string Current { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 确认新密码
        /// </summary>
        public string Confirm { get; set; }
    }
}
