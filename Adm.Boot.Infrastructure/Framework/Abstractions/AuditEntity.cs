using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Adm.Boot.Infrastructure.Domain;
using Adm.Boot.Infrastructure.Extensions;

namespace Adm.Boot.Infrastructure.Framework.Abstractions {
    public class AuditEntity : AuditEntity<int> {
    }

    public class AuditEntity<TKey> : CreationEntity<TKey> {
        public TKey ModifierId { get; set; }

        [MaxLength(EntityDefault.LongNameLength)]
        public string ModifierName { get; set; }

        public DateTime? ModifyTime { get; set; }
    }
}
