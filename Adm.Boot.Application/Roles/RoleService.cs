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
            var cacheKey = "ROLE_URI";
            var cachObj = _cache.GetObject<List<GetRoleUriOutput>>(cacheKey);
            if (cachObj != null) {
                return cachObj;
            } else {
                var roleUris = (from r in _roleRepository.GetAll()
                                join rm in _roleMenuRepository.GetAll()
                                on r.Id equals rm.RoleId
                                join m in _menuRepository.GetAll()
                                on rm.MenuId equals m.Id
                                where r.Status == SysStatus.有效 && m.Status == SysStatus.有效 && m.MenuType == MenuType.按钮
                                select new GetRoleUriOutput {
                                    RoleId = r.Id,
                                    Uri = m.Uri
                                }).ToList();

                _cache.SetObject(cacheKey, roleUris, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(30) });
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
    }
}
