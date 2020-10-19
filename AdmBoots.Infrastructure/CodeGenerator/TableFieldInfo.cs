using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Infrastructure.Framework.Abstractions;

namespace AdmBoots.Infrastructure.CodeGenerator {

    public class TableFieldInfo {
        public string TableColumn { get; set; }

        public string Datatype { get; set; }

        public string FieldLength { get; set; }

        public string IsNullable { get; set; }
        public string TableIdentity { get; set; }
        public string FieldDefault { get; set; }
        public string Remark { get; set; }
    }
}
