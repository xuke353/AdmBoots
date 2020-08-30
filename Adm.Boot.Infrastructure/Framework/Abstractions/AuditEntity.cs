using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using AdmBoots.Infrastructure.Domain;
using AdmBoots.Infrastructure.Extensions;

namespace AdmBoots.Infrastructure.Framework.Abstractions {
    public class AuditEntity : AuditEntity<int> {
    }

    public class AuditEntity<TKey> : CreationEntity<TKey> {
        public TKey ModifierId { get; set; }

        [MaxLength(EntityDefault.LongNameLength)]
        public string ModifierName { get; set; }

        public DateTime? ModifyTime { get; set; }
    }
}
