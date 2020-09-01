using System;
using System.Collections.Generic;
using System.Text;

namespace AdmBoots.Application.Roles.Dto {
    public class AddOrUpdateRoleInput {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
