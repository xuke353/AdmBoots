using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Infrastructure.Framework.Abstractions;

namespace AdmBoots.Application.Users.Dto {
    public class GetUserInput : PageRequest {
        public string Name { get; set; }
    }
}
