using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Application.Roles.Dto;

namespace AdmBoots.Application.Roles {
    public interface IRoleService {

        IList<GetRoleUriOutput> GetRoleUriMaps();

        IEnumerable<GetTransferRoleOutput> GetTransferRoles();
    }
}
