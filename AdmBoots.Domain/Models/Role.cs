using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using AdmBoots.Infrastructure.Framework.Abstractions;

namespace AdmBoots.Domain.Models {
    [Table("role")]
    public class Role : AuditEntity {

        public string Name { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public SysStatus Status { get; set; }

        //多对多映射
        public List<UserRole> UserRoleList { get; set; } = new List<UserRole>();
        public List<RoleMenu> RoleMenuList { get; set; } = new List<RoleMenu>();

    }
}
