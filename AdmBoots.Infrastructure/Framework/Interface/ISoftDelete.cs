using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmBoots.Infrastructure.Framework.Interface {
    public interface ISoftDelete<TKey> {

        bool IsDeleted { get; set; }

        TKey DeleterId { get; set; }

        DateTime? DeletionTime { get; set; }
    }

    public interface ISoftDelete : ISoftDelete<int?> {
    }
}
