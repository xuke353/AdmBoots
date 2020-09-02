using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Domain;
using AdmBoots.Domain.Models;

namespace AdmBoots.Application.Menus.Dto {
    public class GetTreeMenuOutput : Menu {
        /// <summary>
        /// 路由/资源标识
        /// </summary>
        public string Path => Uri;
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool HideInMenu => !IsActive;
        /// <summary>
        /// 父菜单数组
        /// </summary>
        public List<int> ParentIdList { get; set; } = new List<int>();


        public IList<GetTreeMenuOutput> Children { get; set; }

    }
}
