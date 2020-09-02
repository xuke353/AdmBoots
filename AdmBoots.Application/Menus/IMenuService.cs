using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Application.Menus.Dto;

namespace AdmBoots.Application.Menus {
    public interface IMenuService : ITransientDependency {
        IEnumerable<GetTreeMenuOutput> GetMenusByRole();
        IEnumerable<GetTreeMenuOutput> GetActiveMenus();
        IEnumerable<GetTreeMenuOutput> GetAllMenus(GetMenuInput input);
        IEnumerable<GetTreeMenuOutput> GetCascadeMenus();

        void AddOrUpdateMenu(int? id, AddOrUpdateMenuInput input);
        void DeleteMenu(int[] ids);
    }
}
