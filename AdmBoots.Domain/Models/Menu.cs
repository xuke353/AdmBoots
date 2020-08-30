using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using AdmBoots.Infrastructure.Domain;
using AdmBoots.Infrastructure.Framework.Abstractions;

namespace AdmBoots.Domain.Models {
    [Table("menu")]
    public class Menu : AuditEntity {
        /// <summary>
        /// 菜单名称
        /// </summary>
        [Required, MaxLength(EntityDefault.FieldsLength50)]
        public string Name { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        [Required, MaxLength(EntityDefault.FieldsLength50)]
        public string Code { get; set; }

        /// <summary>
        /// 菜单icon
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength50)]
        public string Icon { get; set; }

        /// <summary>
        /// 资源标识
        /// </summary>
        [Required, MaxLength(EntityDefault.FieldsLength50)]
        public string Uri { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        [Required]
        public MenuType MenuType { get; set; }

        /// <summary>
        /// 父id
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Required, MaxLength(EntityDefault.FieldsLength500)]
        public string Description { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public int IsActive { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Required]
        public SysStatus Status { get; set; } = SysStatus.有效;
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

    }
}
