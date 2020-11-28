using System;
using System.Collections.Generic;
using System.Text;

namespace AdmBoots.Infrastructure.Framework.Interface {
    public interface IEntity<TPrimaryKey> {
        TPrimaryKey Id { get; set; }

        bool IsTransient();
    }
}
