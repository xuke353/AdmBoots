using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Domain;

namespace AdmBoots.Application.Menus.Dto {
    public class AddOrUpdateMenuInput {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 菜单别名
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 路由
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// 菜单icon
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 是否为菜单
        /// </summary>
        public MenuType MenuType { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        ///id集合,最后一个ID为父亲ID
        /// </summary>
        public int?[] ParentIdList { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int Sort { get; set; }
    }
}
