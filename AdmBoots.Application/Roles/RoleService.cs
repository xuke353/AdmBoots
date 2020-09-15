using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Application.Roles.Dto;
using AdmBoots.Domain.IRepositories;
using AdmBoots.Domain.Models;
using AdmBoots.Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using AdmBoots.Domain;
using AdmBoots.Data.EntityFrameworkCore.Uow;
using System.Threading.Tasks;
using AdmBoots.Infrastructure.Framework.Abstractions;
using Microsoft.EntityFrameworkCore;
using AdmBoots.Infrastructure.CustomExceptions;
using AdmBoots.Infrastructure.Domain;

namespace AdmBoots.Application.Roles {

    public class RoleService : AppServiceBase, IRoleService {
        private readonly IRepository<Role, int> _roleRepository;
        private readonly IRepository<RoleMenu, int> _roleMenuRepository;
        private readonly IRepository<Menu, int> _menuRepository;
        private readonly IDistributedCache _cache;

        public RoleService(IRepository<Role, int> roleRepository,
            IRepository<RoleMenu, int> roleMenuRepository,
            IRepository<Menu, int> menuRepository,
            IDistributedCache cache) {
            _roleRepository = roleRepository;
            _roleMenuRepository = roleMenuRepository;
            _menuRepository = menuRepository;
            _cache = cache;
        }

        /// <summary>
        /// 获取角色与资源标识（鉴权用）
        /// </summary>
        /// <returns></returns>
        [UnitOfWork(IsDisabled = true)]
        public IList<GetRoleUriOutput> GetRoleUriMaps() {
            var cachObj = _cache.GetObject<List<GetRoleUriOutput>>(AdmConsts.ROLE_URI_CACHE);
            if (cachObj != null) {
                return cachObj;
            } else {
                var roleUris = (from r in _roleRepository.GetAll()
                                join rm in _roleMenuRepository.GetAll()
                                on r.Id equals rm.RoleId
                                join m in _menuRepository.GetAll()
                                on rm.MenuId equals m.Id
                                where r.Status == SysStatus.有效 && m.Status == SysStatus.有效 && m.MenuType == MenuType.按钮 && m.IsActive
                                select new GetRoleUriOutput {
                                    RoleId = r.Id,
                                    Uri = m.Uri
                                }).ToList();

                _cache.SetObject(AdmConsts.ROLE_URI_CACHE, roleUris, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(30) });
                return roleUris;
            }
        }

        [UnitOfWork(IsDisabled = true)]
        public IEnumerable<GetTransferRoleOutput> GetTransferRoles() {
            var roles = _roleRepository.GetAll().Where(t => t.Status == SysStatus.有效)
             .Select(r => new GetTransferRoleOutput { RoleId = r.Id, RoleName = r.Name })
             .ToList();
            return roles;
        }

        [UnitOfWork(IsDisabled = true)]
        public Page<GetRoleOutput> GetRoleList(GetRoleInput input) {
            var result = _roleRepository.GetAll().Where(t => t.Status == SysStatus.有效)
                .WhereIf(!string.IsNullOrEmpty(input.Name), t => t.Name.Contains(input.Name) || t.Code.Contains(input.Name))
                .Include(u => u.RoleMenuList);
            var pageResult = result.PageAndOrderBy(input);
            var output = new List<GetRoleOutput>();
            foreach (var r in pageResult) {
                var userOutput = ObjectMapper.Map<GetRoleOutput>(r);
                foreach (var rm in r.RoleMenuList) {
                    userOutput.MenuIds.Add(rm.MenuId);
                }
                output.Add(userOutput);
            }
            return new Page<GetRoleOutput>(input, result.Count(), output);
        }

        public async Task AddOrUpdateRole(int? id, AddOrUpdateRoleInput input) {
            if (id.HasValue) {
                var checkRole = _roleRepository.GetAll().Where(t => t.Id != id && (t.Name == input.Name || t.Code == input.Code) && t.Status == SysStatus.有效);
                if (checkRole.Any()) {
                    throw new BusinessException($"角色编号或角色名称已存在");
                }
                var role = _roleRepository.GetAll().Where(t => t.Id == id).FirstOrDefault();
                if (role == null)
                    throw new BusinessException($"找不到待更新的数据 ID：{id}");

                role.Name = input.Name;
                role.Code = input.Code;
                role.Description = input.Description;
                await _roleRepository.UpdateAsync(role);
            } else {
                var checkUserName = _roleRepository.GetAll().Where(t => (t.Name == input.Name || t.Code == input.Code) && t.Status == SysStatus.有效);
                if (checkUserName.Any()) {
                    throw new BusinessException($"角色编号或角色名称已存在");
                }
                var role = new Role {
                    Name = input.Name,
                    Code = input.Code,
                    Status = SysStatus.有效,
                    Description = input.Description,
                };
                await _roleRepository.InsertAsync(role);
            }
        }

        public async Task DeleteRole(int[] ids) {
            var updateRoles = await _roleRepository.GetAll().Where(t => ids.Contains(t.Id)).ToListAsync();
            foreach (var role in updateRoles) {
                role.Status = SysStatus.作废;
            }
        }

        public async Task UpdateRoleMenu(UpdateRoleMenuInput input) {
            var roleId = input.RoleId;
            _roleMenuRepository.Delete(t => t.RoleId == roleId);
            var roleMenus = new List<RoleMenu>();
            foreach (var menuId in input.MenuIds) {
                await _roleMenuRepository.InsertAsync(new RoleMenu { RoleId = roleId, MenuId = menuId });
            }
            _cache.Remove(AdmConsts.ROLE_URI_CACHE);
        }
    }
}
