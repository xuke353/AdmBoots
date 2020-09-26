using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdmBoots.Application.Menus.Dto;
using AdmBoots.Domain;
using AdmBoots.Domain.IRepositories;
using AdmBoots.Domain.Models;
using AdmBoots.Infrastructure.CustomExceptions;
using AdmBoots.Infrastructure.Domain;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq.Dynamic.Core;
using AdmBoots.Data.EntityFrameworkCore.Uow;

namespace AdmBoots.Application.Menus {

    public class MenuService : AppServiceBase, IMenuService {
        private readonly IRepository<Menu, int> _menuRepository;
        private readonly IRepository<UserRole, int> _userRoleRepository;
        private readonly IRepository<Role, int> _roleRepository;
        private readonly IRepository<RoleMenu, int> _roleMenuRepository;
        private readonly IDistributedCache _cache;
        private readonly IRepository<User, int> _userRepository;

        public MenuService(IRepository<Menu, int> menuRepository,
            IRepository<UserRole, int> userRoleRepository,
            IRepository<Role, int> roleRepository,
            IRepository<RoleMenu, int> roleMenuRepository,
            IDistributedCache cache,
            IRepository<User, int> userRepository) {
            _menuRepository = menuRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _roleMenuRepository = roleMenuRepository;
            _cache = cache;
            _userRepository = userRepository;
        }

        public void AddOrUpdateMenu(int? id, AddOrUpdateMenuInput input) {
            if (id.HasValue) {
                var parentMenu = input.ParentIdList;
                var menu = _menuRepository.FirstOrDefault(t => t.Id == id);
                if (menu == null) {
                    throw new BusinessException($"未找到数据");
                }
                var hasUri = _menuRepository.GetAll().Any(t => t.Uri == input.Uri.Trim() && input.MenuType == t.MenuType && id != t.Id);
                if (hasUri) {
                    throw new BusinessException($"资源标识重复，请修改");
                }
                var hasCode = _menuRepository.GetAll().Any(t => t.Code == input.Code && input.MenuType == t.MenuType && id != t.Id);
                if (hasCode) {
                    throw new BusinessException($"菜单编号识重复，请修改");
                }
                var updateEntity = ObjectMapper.Map(input, menu);
                updateEntity.IsActive = input.IsActive;
                updateEntity.ParentId = parentMenu == null || parentMenu.Length < 1 ? null : parentMenu.Last();
                _menuRepository.Update(updateEntity);

                _cache.Remove(AdmConsts.ROLE_URI_CACHE);
            } else {
                var hasUri = _menuRepository.GetAll().Any(t => t.Uri == input.Uri.Trim() && input.MenuType == t.MenuType);
                if (hasUri) {
                    throw new BusinessException($"资源标识重复，请修改");
                }
                var hasCode = _menuRepository.GetAll().Any(t => t.Code == input.Code && input.MenuType == t.MenuType);
                if (hasCode) {
                    throw new BusinessException($"菜单编号识重复，请修改");
                }
                var parentMenu = input.ParentIdList;
                var menu = ObjectMapper.Map<Menu>(input);
                menu.Status = SysStatus.有效;
                menu.Uri = input.Uri.Trim();
                menu.ParentId = parentMenu == null || parentMenu.Length < 1 ? null : parentMenu.Last();
                _menuRepository.Insert(menu);
            }
        }

        public void DeleteMenu(int[] ids) {
            var menus = _menuRepository.GetAll().ToList();
            foreach (var menu in menus.Where(t => ids.Contains(t.Id))) {
                menu.Status = SysStatus.作废;
                _menuRepository.Update(menu);
                DeleteMenuChild(menus, menu.Id);
            }
        }

        private void DeleteMenuChild(List<Menu> menus, int id) {
            var chidMenus = menus.Where(m => m.ParentId == id);
            foreach (var menu in chidMenus) {
                menu.Status = SysStatus.作废;
                _menuRepository.Update(menu);
                DeleteMenuChild(menus, menu.Id);
            }
        }

        /// <summary>
        /// 获取激活的菜单与按钮（给角色授权时）
        /// </summary>
        /// <returns></returns>
        [UnitOfWork(IsDisabled = true)]
        public IEnumerable<GetTreeMenuOutput> GetActiveMenus() {
            var menus = _menuRepository.GetAll()
                .Where(t => t.IsActive && t.Status == SysStatus.有效).OrderBy(t => t.Sort).ToList();
            var menusOutput = new List<GetTreeMenuOutput>();
            foreach (var menu in menus.Where(t => t.ParentId == -1)) {
                var menuMap = ObjectMapper.Map<GetTreeMenuOutput>(menu);
                menuMap.Children = GetChildrens(menus, menuMap);
                menusOutput.Add(menuMap);
            }
            return menusOutput;
        }

        /// <summary>
        /// 获取所有菜单与按钮（菜单管理）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [UnitOfWork(IsDisabled = true)]
        public IEnumerable<GetTreeMenuOutput> GetAllMenus(GetMenuInput input) {
            var menus = _menuRepository.GetAll()
                .Where(t => t.Status == SysStatus.有效)
                .OrderBy("Sort").ToList();
            var menusOutput = new List<GetTreeMenuOutput>();
            //有查询条件时，不构造树形结构
            if (!string.IsNullOrWhiteSpace(input.Name)) {
                var whereMenus = menus.Where(t => t.Name.Contains(input.Name) || t.Code.Contains(input.Name));
                foreach (var menu in whereMenus) {
                    var menuMap = ObjectMapper.Map<GetTreeMenuOutput>(menu);
                    GetParentId(menus, menu, menuMap.ParentIdList);
                    menuMap.Children = null;
                    menusOutput.Add(menuMap);
                }
            } else {
                foreach (var menu in menus.Where(t => t.ParentId == -1)) {
                    var menuMap = ObjectMapper.Map<GetTreeMenuOutput>(menu);
                    GetParentId(menus, menu, menuMap.ParentIdList);
                    menuMap.Children = GetChildrens(menus, menuMap);
                    menusOutput.Add(menuMap);
                }
            }
            return menusOutput;
        }

        /// <summary>
        /// 获取级联形式的菜单（菜单管理编辑时，选取父菜单）
        /// </summary>
        /// <returns></returns>
        [UnitOfWork(IsDisabled = true)]
        public IEnumerable<GetTreeMenuOutput> GetCascadeMenus() {
            var menus = _menuRepository.GetAll().Where(t => t.MenuType == MenuType.菜单
                        && t.IsActive
                        && t.Status == SysStatus.有效).OrderBy("Sort").ToList();
            var menusOutput = new List<GetTreeMenuOutput>();
            var root = new Menu {
                Id = -1,
                Name = "根节点",
                Code = "Root",
                Uri = "/",
                MenuType = MenuType.菜单,
                ParentId = null
            };
            menus.Add(root);
            foreach (var menu in menus.Where(t => t.ParentId == null)) {
                var menuMap = ObjectMapper.Map<GetTreeMenuOutput>(menu);
                menuMap.Children = GetChildrens(menus, menuMap);
                menusOutput.Add(menuMap);
            }
            return menusOutput;
        }

        /// <summary>
        /// 根据当前用户角色获取系统左侧菜单
        /// </summary>
        /// <returns></returns>
        [UnitOfWork(IsDisabled = true)]
        public IEnumerable<GetTreeMenuOutput> GetMenusByRole() {
            var menus = (from u in _userRepository.GetAll()
                         join ur in _userRoleRepository.GetAll()
                         on u.Id equals ur.UserId
                         join r in _roleRepository.GetAll()
                         on ur.RoleId equals r.Id
                         join rm in _roleMenuRepository.GetAll()
                         on r.Id equals rm.RoleId
                         join m in _menuRepository.GetAll()
                         on rm.MenuId equals m.Id
                         where r.Status == SysStatus.有效 && m.Status == SysStatus.有效 && m.MenuType == MenuType.菜单 && u.Id == AdmSession.UserId && m.IsActive
                         select m).OrderBy(t => t.Sort).ToList();
            var menusOutput = new List<GetTreeMenuOutput>();
            foreach (var menu in menus.Where(t => t.ParentId == -1)) {
                var menuMap = ObjectMapper.Map<GetTreeMenuOutput>(menu);
                menuMap.Children = GetChildrens(menus, menuMap);
                menusOutput.Add(menuMap);
            }
            return menusOutput;
        }

        private IList<GetTreeMenuOutput> GetChildrens(IList<Menu> menus, GetTreeMenuOutput childMenu) {
            var childrens = menus.Where(m => m.ParentId == childMenu.Id).ToList();
            var menusOutput = new List<GetTreeMenuOutput>();
            foreach (var menu in childrens) {
                var menuMap = ObjectMapper.Map<GetTreeMenuOutput>(menu);
                GetParentId(menus, menu, menuMap.ParentIdList);
                menuMap.Children = GetChildrens(menus, menuMap);
                menusOutput.Add(menuMap);
            }
            return menusOutput.Count > 0 ? menusOutput : null;
        }

        private void GetParentId(IList<Menu> menus, Menu child, List<int> parentIdList) {
            var menu = menus.FirstOrDefault(t => t.Id == child.ParentId);
            if (menu != null) {
                parentIdList.Insert(0, menu.Id);
                GetParentId(menus, menu, parentIdList);
            } else {
                if (child.ParentId.HasValue)
                    parentIdList.Insert(0, (int)child.ParentId);
            }
        }
    }
}
