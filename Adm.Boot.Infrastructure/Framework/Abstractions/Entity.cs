using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Adm.Boot.Infrastructure.Framework.Abstractions
{
    [Serializable]
    public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
    {

        /// <summary>
        /// EFCore 主键设置
        /// 1:约定 名为Id或者类名Id这个属性设置为主键 
        /// 2:注解 [Key]
        /// 3:Fluent API modelBuilder.Entity<Car>().HasKey(c => new { c.State, c.LicensePlate});
        /// </summary>
        [Key]
        public virtual TPrimaryKey Id { get; set; }

        public virtual bool IsTransient()
        {
            if (EqualityComparer<TPrimaryKey>.Default.Equals(Id, default(TPrimaryKey)))
            {
                return true;
            }
            if (typeof(TPrimaryKey) == typeof(int))
            {
                return Convert.ToInt32(Id) <= 0;
            }
            if (typeof(TPrimaryKey) == typeof(long))
            {
                return Convert.ToInt64(Id) <= 0;
            }
            return false;
        }

    }


}
