using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Adm.Boot.Infrastructure.Domain;
using Adm.Boot.Infrastructure.Extensions;

namespace Adm.Boot.Infrastructure.Framework.Abstractions {

    public class CreationEntity : CreationEntity<int> { }

    public class CreationEntity<TKey> : Entity<TKey> {
        public TKey CreatorId { get; set; }

        [MaxLength(EntityDefault.LongNameLength)]
        public string CreatorName { get; set; }

        public virtual DateTime? CreateTime { get; set; }
    }
}
