using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using AdmBoots.Infrastructure.Domain;
using AdmBoots.Infrastructure.Extensions;

namespace AdmBoots.Infrastructure.Framework.Abstractions {

    public class CreationEntity : CreationEntity<int> { }

    public class CreationEntity<TKey> : Entity<TKey> {
        public TKey CreatorId { get; set; }

        [MaxLength(EntityDefault.LongNameLength)]
        public string CreatorName { get; set; }

        public virtual DateTime? CreateTime { get; set; }
    }
}
