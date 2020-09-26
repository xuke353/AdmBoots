using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using AdmBoots.Infrastructure.Domain;
using AdmBoots.Infrastructure.Framework.Abstractions;

namespace AdmBoots.Domain.Models {
    /// <summary>
    /// 演示用，无意义
    /// </summary>
    [Table("Test")]
    public class Test : AuditEntity {

        [Required, MaxLength(EntityDefault.FieldsLength50)]
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
