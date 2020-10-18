using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AdmBoots.Infrastructure.CodeGenerator {
    public class CodeGeneratorInput {

        /// <summary>
        /// 类名前缀
        /// </summary>
        [Required]
        public string ClassPrefix { get; set; }
        /// <summary>
        /// 数据库表名(默认使用ClassPrefix)
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string ClassDescription { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string CreateName { get; set; }


        /// <summary>
        /// 创建Model
        /// </summary>
        public bool WriteModel { get; set; } = true;

        /// <summary>
        /// 创建Cotroller
        /// </summary>
        public bool WriteCotroller { get; set; } = true;

        /// <summary>
        /// 创建Service
        /// </summary>
        public bool WriteService { get; set; } = true;

        /// <summary>
        /// 创建DTO
        /// </summary>
        public bool WriteDto { get; set; } = true;
    }
}
