using System;
using System.Collections.Generic;
using System.Text;

namespace AdmBoots.Infrastructure.CodeGenerator {

    public class FileConfigModel {
        public string ClassPrefix { get; set; }
        public string TableName { get; set; }
        public string ClassDescription { get; set; }
        public string CreateName { get; set; }
        public string CreateDate { get; set; }
        public string EntityName { get; set; }
        public string ServiceName { get; set; }
        public string IServiceName { get; set; }
        public string ControllerName { get; set; }
        public string DtoGetInputName { get; set; }
        public string DtoGetOutputName { get; set; }
        public string DtoUpdateInputName { get; set; }

        public string OutputEntity { get; set; }
        public string OutputService { get; set; }
        public string OutputDto { get; set; }
        public string OutputController { get; set; }
    }
}
