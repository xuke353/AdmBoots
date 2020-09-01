using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AdmBoots.Application.Roles.Dto;
using AdmBoots.Infrastructure.Framework.Abstractions;

namespace AdmBoots.Application.Roles {
    public interface IRoleService : ITransientDependency {

        IList<GetRoleUriOutput> GetRoleUriMaps();

        IEnumerable<GetTransferRoleOutput> GetTransferRoles();

        Page<GetRoleOutput> GetRoleList(GetRoleInput input);

        Task AddOrUpdateRole(int? id, AddOrUpdateRoleInput input);

        Task DeleteRole(int[] ids);

        Task UpdateRoleMenu(UpdateRoleMenuInput input);
    }
}
