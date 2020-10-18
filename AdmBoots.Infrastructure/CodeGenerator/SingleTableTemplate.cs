using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace AdmBoots.Infrastructure.CodeGenerator {
    public class SingleTableTemplate {

        public FileConfigModel GetFileConfig(string path, CodeGeneratorInput input) {
            path = path.Trim('\\');
            path = Directory.GetParent(path).FullName;
            var fileConfig = new FileConfigModel {
                ClassPrefix = input.ClassPrefix,
                ClassDescription = input.ClassDescription,
                CreateDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                CreateName = input.CreateName,
                EntityName = input.ClassPrefix,
                TableName = string.IsNullOrWhiteSpace(input.TableName) ? input.ClassPrefix : input.TableName,
                IServiceName = $"I{input.ClassPrefix}Service",
                ServiceName = $"{input.ClassPrefix}Service",
                ControllerName = $"{input.ClassPrefix}Controller",
                DtoGetInputName = $"Get{input.ClassPrefix}Input",
                DtoGetOutName = $"Get{input.ClassPrefix}Output",
            };
            fileConfig.OutputController = Path.Combine(path, "AdmBoots.Api", "Controllers");
            fileConfig.OutputService = Path.Combine(path, "AdmBoots.Application", $"{fileConfig.ServiceName}s");
            fileConfig.OutputDto = Path.Combine(fileConfig.ServiceName, "Dto");
            fileConfig.OutputEntity = Path.Combine(path, "AdmBoots.Domain", "Models");
            return fileConfig;
        }


        #region BuildEntity
        public string BuildEntity(FileConfigModel baseConfigModel, DataTable dt) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using Newtonsoft.Json;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
            sb.AppendLine("using YiSha.Util;");
            sb.AppendLine();

            sb.AppendLine("namespace AdmBoots.Domain.Models");
            sb.AppendLine("{");

            SetClassDescription("实体类", baseConfigModel, sb);

            sb.AppendLine("    [Table(\"" + baseConfigModel.TableName + "\")]");
            sb.AppendLine("    public class " + baseConfigModel.EntityName + " : " + GetBaseEntity(dt));
            sb.AppendLine("    {");

            string column = string.Empty;
            string remark = string.Empty;
            string datatype = string.Empty;
            string isNullable = string.Empty;
            foreach (DataRow dr in dt.Rows) {
                column = dr["TableColumn"].ToString();
                if (BaseField.BaseFieldList.Where(p => p == column).Any()) {
                    // 基础字段不需要生成，继承合适的BaseEntity即可。
                    continue;
                }

                remark = dr["Remark"].ToString();
                datatype = dr["Datatype"].ToString();
                isNullable = dr["IsNullable"].ToString();
                datatype = GetPropertyDatatype(datatype, isNullable);

                sb.AppendLine("        /// <summary>");
                sb.AppendLine("        /// " + remark);
                sb.AppendLine("        /// </summary>");
                sb.AppendLine("        /// <returns></returns>");

                switch (datatype) {
                    case "long?":
                        sb.AppendLine("        [JsonConverter(typeof(StringJsonConverter))]");
                        break;
                    case "long":
                        sb.AppendLine("        [JsonConverter(typeof(StringJsonConverter))]");
                        break;
                    case "DateTime?":
                        sb.AppendLine("        [JsonConverter(typeof(DateTimeJsonConverter))]");
                        break;
                    case "DateTime":
                        sb.AppendLine("        [JsonConverter(typeof(DateTimeJsonConverter))]");
                        break;
                }
                sb.AppendLine("        public " + datatype + " " + column + " { get; set; }");
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
        #endregion

        #region SetClassDescription
        private void SetClassDescription(string type, FileConfigModel baseConfigModel, StringBuilder sb) {
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// 创 建：" + baseConfigModel.CreateName);
            sb.AppendLine("    /// 日 期：" + baseConfigModel.CreateDate);
            sb.AppendLine("    /// 描 述：" + baseConfigModel.ClassDescription + type);
            sb.AppendLine("    /// </summary>");
        }
        #endregion

        private string GetBaseEntity(DataTable dt) {
            string entity = string.Empty;
            var columnList = dt.AsEnumerable().Select(p => p["TableColumn"].ToString()).ToList();

            bool id = columnList.Where(p => p == "Id").Any();
            bool baseIsDelete = columnList.Where(p => p == "BaseIsDelete").Any();
            bool baseVersion = columnList.Where(p => p == "RowVersion").Any();
            bool baseModifyTime = columnList.Where(p => p == "ModifyTime").Any();
            bool baseModifierId = columnList.Where(p => p == "ModifierId").Any();
            bool baseModifierName = columnList.Where(p => p == "ModifierName").Any();
            bool baseCreateTime = columnList.Where(p => p == "CreateTime").Any();
            bool baseCreatorId = columnList.Where(p => p == "CreatorId").Any();
            bool baseCreatorName = columnList.Where(p => p == "CreatorName").Any();

            if (!id) {
                throw new Exception("数据库表必须有主键Id字段");
            }
            if (baseModifyTime && baseModifierId && baseModifierName && baseCreateTime && baseCreatorId && baseCreatorName) {
                entity = "AuditEntity";
            } else if (baseCreateTime && baseCreatorId && baseCreatorName) {
                entity = "CreationEntity";
            } else {
                entity = "Entity";
            }
            return entity;
        }

        public static string GetPropertyDatatype(string sDatatype, string sIsNullable) {
            string sTempDatatype = string.Empty;
            sDatatype = sDatatype.ToLower();
            sIsNullable = sIsNullable.ToUpper();
            switch (sDatatype) {
                case "int":
                case "number":
                case "integer":
                case "smallint":
                    sTempDatatype = sIsNullable == "N" ? "int" : "int?";
                    break;

                case "bigint":
                    sTempDatatype = sIsNullable == "N" ? "long" : "long?";
                    break;

                case "tinyint":
                    sTempDatatype = "byte?";
                    break;

                case "numeric":
                case "real":
                    sTempDatatype = sIsNullable == "N" ? "Single" : "Single?";
                    break;

                case "float":
                    sTempDatatype = sIsNullable == "N" ? "float" : "float?";
                    break;

                case "decimal":
                case "numer(8,2)":
                    sTempDatatype = sIsNullable == "N" ? "decimal" : "decimal?";
                    break;

                case "bit":
                    sTempDatatype = sIsNullable == "N" ? "bool" : "bool?";
                    break;

                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    sTempDatatype = sIsNullable == "N" ? "DateTime" : "DateTime?";
                    break;

                case "money":
                case "smallmoney":
                    sTempDatatype = sIsNullable == "N" ? "double" : "double?";
                    break;

                case "char":
                case "varchar":
                case "nvarchar2":
                case "text":
                case "nchar":
                case "nvarchar":
                case "ntext":
                default:
                    sTempDatatype = "string";
                    break;
            }
            return sTempDatatype;
        }
    }

    public class BaseField {
        public static string[] BaseFieldList = new string[]
        {
            "Id",
            "BaseIsDelete",
            "ModifierId",
            "ModifierName",
            "ModifyTime",
            "CreatorId",
            "CreatorName",
            "CreateTime",
            "RowVersion"
        };
    }
}
