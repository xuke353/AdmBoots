using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Adm.Boot.Infrastructure.Framework.Abstractions;

namespace Adm.Boot.Domain.Models {
    [Table("role")]
    public class Role : AuditEntity {

        public string Name { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public SysStatus Status { get; set; }

    }
}
