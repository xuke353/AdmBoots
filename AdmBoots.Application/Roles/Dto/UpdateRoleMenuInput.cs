using System;
using System.Collections.Generic;
using System.Text;

namespace AdmBoots.Application.Roles.Dto {
    public class UpdateRoleMenuInput {
        public int RoleId { get; set; }
        public List<int> MenuIds { get; set; }
    }
}
