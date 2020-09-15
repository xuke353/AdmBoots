using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using AdmBoots.Infrastructure.Domain;
using AdmBoots.Infrastructure.Framework.Abstractions;

namespace AdmBoots.Domain.Models {

    [Table("role")]
    public class Role : AuditEntity {

        [Required, MaxLength(EntityDefault.FieldsLength50)]
        public string Name { get; set; }

        [MaxLength(EntityDefault.FieldsLength50)]
        public string Code { get; set; }

        [MaxLength(EntityDefault.FieldsLength500)]
        public string Description { get; set; }

        [Required]
        public SysStatus Status { get; set; }

        //多对多映射
        public List<UserRole> UserRoleList { get; set; } = new List<UserRole>();

        public List<RoleMenu> RoleMenuList { get; set; } = new List<RoleMenu>();
    }
}
