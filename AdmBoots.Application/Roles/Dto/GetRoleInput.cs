using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Infrastructure.Framework.Abstractions;

namespace AdmBoots.Application.Roles.Dto {
    public class GetRoleInput : PageRequest {
        public string Name { get; set; }
    }
}
