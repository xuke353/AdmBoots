using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Domain;

namespace AdmBoots.Application.Users.Dto {
    public class AddOrUpdateUserInput {

        public string UserName { get; set; }

        public string Name { get; set; }

        public SysStatus Status { get; set; }

        public string Email { get; set; }
        /// <summary>
        /// roleId
        /// </summary>
        public List<int> Roles { get; set; }
    }
}
