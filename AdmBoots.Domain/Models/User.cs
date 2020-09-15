using AdmBoots.Infrastructure.Domain;
using AdmBoots.Infrastructure.Framework.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using AdmBoots.Domain;

namespace AdmBoots.Domain.Models {

    [Table("user")]
    public class User : Entity {

        /// <summary>
        /// 用户名
        /// </summary>
        [Required, MaxLength(EntityDefault.FieldsLength50)]
        public string UserName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required, MaxLength(EntityDefault.FieldsLength50)]
        public string Name { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required, MaxLength(EntityDefault.FieldsLength50)]
        public string Password { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Required]
        public SysStatus Status { get; set; } = SysStatus.有效;

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 是否为系统管理员
        /// </summary>
        public bool IsMaster { get; set; } = false;

        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength50)]
        public string Email { get; set; }

        //多对多映射
        public List<UserRole> UserRoleList { get; set; } = new List<UserRole>();
    }
}
