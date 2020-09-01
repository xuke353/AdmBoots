using System;
using System.ComponentModel.DataAnnotations;
using AdmBoots.Infrastructure.Domain;

namespace AdmBoots.Infrastructure.Framework.Abstractions {
    public class AuditEntity : CreationEntity<int> {
        public int? ModifierId { get; set; }

        [MaxLength(EntityDefault.LongNameLength)]
        public string ModifierName { get; set; }

        public DateTime? ModifyTime { get; set; }
    }

    public class AuditEntity<TKey> : CreationEntity<TKey> {
        public TKey ModifierId { get; set; }

        [MaxLength(EntityDefault.LongNameLength)]
        public string ModifierName { get; set; }

        public DateTime? ModifyTime { get; set; }
    }
}
