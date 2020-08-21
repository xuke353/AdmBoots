using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Adm.Boot.Infrastructure.Framework.Abstractions;

namespace Adm.Boot.Domain.Models {
    [Table("user_role")]
    public class UserRole : Entity {

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

    }
}
