using System;
using System.Collections.Generic;
using System.Text;

namespace Adm.Boot.Infrastructure.CustomExceptions
{
    public class EntityNotFoundException : Exception
    {
        public Type EntityType { get; set; }

        public object Id { get; set; }

        public EntityNotFoundException(Type entityType, object id)
            : this(entityType, id, null)
        {
        }

        public EntityNotFoundException(Type entityType, object id, Exception innerException)
            : base($"找不到该实体。实体类型: {entityType.FullName}, id: {id}", innerException)
        {
            EntityType = entityType;
            Id = id;
        }
    }
}