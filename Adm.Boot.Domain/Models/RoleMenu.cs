using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Adm.Boot.Infrastructure.Framework.Abstractions;

namespace Adm.Boot.Domain.Models {
    [Table("role_menu")]
    public class RoleMenu : Entity {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 菜单ID
        /// </summary>
        public int MenuId { get; set; }
    }
}
