using System;
using System.Collections.Generic;
using System.Text;

namespace AdmBoots.Infrastructure.Framework.Abstractions
{
    public interface IEntity<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }

        bool IsTransient();
    }
}
