using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Domain;

namespace AdmBoots.Application.Users.Dto {
    public class LoginUserInfo {
        public int Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public SysStatus Status { get; set; } = SysStatus.有效;

        /// <summary>
        /// 是否为系统管理员
        /// </summary>
        public bool IsMaster { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        public IList<UserRoles> Roles { get; set; }
    }

    public class UserRoles {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
