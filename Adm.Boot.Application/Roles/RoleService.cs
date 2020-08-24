using System;
using System.Collections.Generic;
using System.Text;
using Adm.Boot.Application.Roles.Dto;
using Adm.Boot.Domain.IRepositories;
using Adm.Boot.Domain.Models;
using Adm.Boot.Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using Adm.Boot.Domain;

namespace Adm.Boot.Application.Roles {
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
    }
}
