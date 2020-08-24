using System;
using System.Collections.Generic;
using System.Text;
using Adm.Boot.Application.Roles.Dto;

namespace Adm.Boot.Application.Roles {
    public interface IRoleService {

        IList<GetRoleUriOutput> GetRoleUriMaps();
    }
}
