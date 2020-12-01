using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using AdmBoots.Infrastructure.Framework.Interface;

namespace AdmBoots.Infrastructure.Framework.Abstractions {

    [Serializable]
    public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey> {

        /// <summary>
        /// EFCore 主键设置
        /// 1:约定 名为Id或者类名Id这个属性设置为主键
        /// 2:注解 [Key]
        /// 3:Fluent API modelBuilder.Entity<Car>().HasKey(c => new { c.State, c.LicensePlate});
        /// </summary>
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]//EF中，主键是int类型时，会自动认为该字段是自增长
        public virtual TPrimaryKey Id { get; set; }

        public virtual bool IsTransient() {
            if (EqualityComparer<TPrimaryKey>.Default.Equals(Id, default)) {
                return true;
            }
            if (typeof(TPrimaryKey) == typeof(int)) {
                return Convert.ToInt32(Id) <= 0;
            }
            if (typeof(TPrimaryKey) == typeof(long)) {
                return Convert.ToInt64(Id) <= 0;
            }
            return false;
        }
    }

    /// <summary>
    /// 定义默认主键类型为int的实体基类
    /// </summary>
    public abstract class Entity : Entity<int> {
    }
}
