using System;
using System.Collections.Generic;
using System.Text;

namespace Adm.Boot.Infrastructure.Framework.Abstractions
{
    [Serializable]
    public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
    {
        public virtual TPrimaryKey Id { get; set; }
    }

    public interface IEntity<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }
    }
}
