using AdmBoots.Domain;
using AdmBoots.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdmBoots.Application.Users.Dto {
    public class GetUserOutput {
        /// <summary>
        /// 唯一键
        /// </summary>
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
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }
        /// <summary>
        /// 是否为系统管理员
        /// </summary>
        public bool IsMaster { get; set; }
        /// <summary>
        /// 未读消息数
        /// </summary>
        public int UnreadCount { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 用户角色
        /// </summary>
        public List<URole> Roles { get; set; } = new List<URole>();
    }

    public class URole {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
