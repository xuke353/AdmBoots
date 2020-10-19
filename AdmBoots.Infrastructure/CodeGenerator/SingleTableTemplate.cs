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
                DtoGetOutputName = $"Get{input.ClassPrefix}Output",
                DtoUpdateInputName = $"Update{input.ClassPrefix}Input",
            };
            fileConfig.OutputController = Path.Combine(path, "AdmBoots.Api", "Controllers");
            fileConfig.OutputService = Path.Combine(path, "AdmBoots.Application", $"{fileConfig.EntityName}s");
            fileConfig.OutputDto = Path.Combine(fileConfig.OutputService, "Dto");
            fileConfig.OutputEntity = Path.Combine(path, "AdmBoots.Domain", "Models");
            return fileConfig;
        }

        #region BuildEntity

        public string BuildEntity(FileConfigModel baseConfigModel, DataTable dt) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
            sb.AppendLine("using AdmBoots.Infrastructure.Framework.Abstractions;");
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

        #endregion BuildEntity

        #region BuildService

        public string BuildService(FileConfigModel baseConfigModel) {
            //小写类名
            string lowerclassName = baseConfigModel.ClassPrefix.Substring(0, 1).ToLower() + baseConfigModel.ClassPrefix.Substring(1);
            StringBuilder sb = new StringBuilder();
            string method = string.Empty;
            sb.AppendLine("using AdmBoots.Domain.Models;");
            sb.AppendLine("using System.Linq.Expressions;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using AdmBoots.Application.{baseConfigModel.ClassPrefix}s.Dto;");
            sb.AppendLine("using AdmBoots.Domain.IRepositories;");
            sb.AppendLine("using AutoMapper;");
            sb.AppendLine("using AdmBoots.Infrastructure.Framework.Abstractions;");

            sb.AppendLine();

            sb.AppendLine($"namespace AdmBoots.Application.{baseConfigModel.ClassPrefix}s");
            sb.AppendLine("{");

            SetClassDescription("服务类", baseConfigModel, sb);

            sb.AppendLine($"    public class {baseConfigModel.ServiceName} : AppServiceBase,I{baseConfigModel.ServiceName}");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly IRepository<{baseConfigModel.EntityName}, int> _{lowerclassName}Repository;");
            sb.AppendLine($"        public {baseConfigModel.ServiceName}(IRepository<{baseConfigModel.EntityName}, int> {lowerclassName}Repository)");
            sb.AppendLine("        {");
            sb.AppendLine($"           _{lowerclassName}Repository = {lowerclassName}Repository;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public Task AddOrUpdate{baseConfigModel.ClassPrefix}(int? id, {baseConfigModel.DtoUpdateInputName} input)");
            sb.AppendLine("        {");
            sb.AppendLine("            throw new System.NotImplementedException();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public Task Delete{baseConfigModel.ClassPrefix}(int[] ids)");
            sb.AppendLine("        {");
            sb.AppendLine("            throw new System.NotImplementedException();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public  Task<Page<{baseConfigModel.DtoGetOutputName}>> Get{baseConfigModel.ClassPrefix}List({baseConfigModel.DtoGetInputName} input)");
            sb.AppendLine("        {");
            sb.AppendLine("            throw new System.NotImplementedException();");
            sb.AppendLine("        }");
            sb.AppendLine();

            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        #endregion BuildService

        #region BuildIService

        public string BuildIService(FileConfigModel baseConfigModel) {
            //小写类名
            string lowerclassName = baseConfigModel.ClassPrefix.Substring(0, 1).ToLower() + baseConfigModel.ClassPrefix.Substring(1);
            StringBuilder sb = new StringBuilder();
            string method = string.Empty;
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using AdmBoots.Application.{baseConfigModel.ClassPrefix}s.Dto;");
            sb.AppendLine("using AdmBoots.Domain.IRepositories;");
            sb.AppendLine("using AdmBoots.Infrastructure.Framework.Abstractions;");

            sb.AppendLine();

            sb.AppendLine($"namespace AdmBoots.Application.{baseConfigModel.ClassPrefix}s");
            sb.AppendLine("{");

            SetClassDescription("服务接口", baseConfigModel, sb);

            sb.AppendLine($"    public interface " + $"I{baseConfigModel.ServiceName} :ITransientDependency");
            sb.AppendLine("    {");
            sb.AppendLine($"        Task<Page<{baseConfigModel.DtoGetOutputName}>> Get{baseConfigModel.ClassPrefix}List({baseConfigModel.DtoGetInputName} input);");
            sb.AppendLine();
            sb.AppendLine($"        Task AddOrUpdate{baseConfigModel.ClassPrefix}(int? id, {baseConfigModel.DtoUpdateInputName} input);");
            sb.AppendLine();
            sb.AppendLine($"        Task Delete{baseConfigModel.ClassPrefix}(int[] ids);");
            sb.AppendLine();

            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        #endregion BuildIService

        #region BuildDto

        public string BuildDto(FileConfigModel baseConfigModel, string dtoName) {
            StringBuilder sb = new StringBuilder();
            if (dtoName.Contains(":"))
                sb.AppendLine("using AdmBoots.Infrastructure.Framework.Abstractions;");

            sb.AppendLine();

            sb.AppendLine($"namespace AdmBoots.Application.{baseConfigModel.ClassPrefix}s.Dto");
            sb.AppendLine("{");

            SetClassDescription("Dto", baseConfigModel, sb);

            sb.AppendLine($"    public class " + $"{dtoName}");
            sb.AppendLine("    {");
            sb.AppendLine();
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        #endregion BuildDto

        #region BuildController

        public string BuildController(FileConfigModel baseConfigModel) {
            string classPrefix = baseConfigModel.ClassPrefix;
            //小写类名
            var lowerClassPrefix = classPrefix.Substring(0, 1).ToLower() + classPrefix.Substring(1);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"using AdmBoots.Application.{classPrefix}s;");
            sb.AppendLine($"using AdmBoots.Application.{classPrefix}s.Dto;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("using System.Web;");
            sb.AppendLine("using Microsoft.AspNetCore.Authorization;");
            sb.AppendLine("using AdmBoots.Infrastructure.Domain;");
            sb.AppendLine("using AdmBoots.Infrastructure.Framework.Abstractions;");
            sb.AppendLine();

            sb.AppendLine("namespace AdmBoots.Api.Controllers");
            sb.AppendLine("{");

            SetClassDescription("控制器类", baseConfigModel, sb);

            sb.AppendLine("    [ApiController]");
            sb.AppendLine("    [ApiVersion(\"1.0\")]");
            sb.AppendLine("    [Route(\"api/v{version:apiVersion}/" + lowerClassPrefix + "s\")]");
            sb.AppendLine("    //[Authorize(AdmConsts.POLICY)]");
            sb.AppendLine("    public class " + baseConfigModel.ControllerName + " : ControllerBase");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{baseConfigModel.ServiceName} _{lowerClassPrefix}Service;");
            sb.AppendLine();
            sb.AppendLine($"        public {baseConfigModel.ControllerName} (I{baseConfigModel.ServiceName} {lowerClassPrefix}Service)");
            sb.AppendLine("        {");
            sb.AppendLine($"            _{lowerClassPrefix}Service = {lowerClassPrefix}Service;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpGet]");
            sb.AppendLine("        public ActionResult Get" + classPrefix + "List([FromQuery]" + baseConfigModel.DtoGetInputName + " input)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var result = _{lowerClassPrefix}Service.Get{classPrefix}List(input);");
            sb.AppendLine($"            return Ok(ResponseBody.From(result));");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpPost(\"{id}\")]");
            sb.AppendLine($"        public async Task<IActionResult> AddOrUpdate{classPrefix}(int? id, [FromBody]{baseConfigModel.DtoUpdateInputName} input)");
            sb.AppendLine("        {");
            sb.AppendLine($"            await _{lowerClassPrefix}Service.AddOrUpdate{classPrefix}(id, input);");
            sb.AppendLine("            return Ok(ResponseBody.From(\"操作成功\"));");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpDelete]");
            sb.AppendLine($"        public async Task<IActionResult> Delete{classPrefix}(int[] ids) ");
            sb.AppendLine("        {");
            sb.AppendLine($"            await _{lowerClassPrefix}Service.Delete{classPrefix}(ids);");
            sb.AppendLine("            return Ok(ResponseBody.From(\"操作成功\"));");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        #endregion BuildController

        #region SetClassDescription

        private void SetClassDescription(string type, FileConfigModel baseConfigModel, StringBuilder sb) {
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// 创 建：" + baseConfigModel.CreateName);
            sb.AppendLine("    /// 日 期：" + baseConfigModel.CreateDate);
            sb.AppendLine("    /// 描 述：" + baseConfigModel.ClassDescription + type);
            sb.AppendLine("    /// </summary>");
        }

        #endregion SetClassDescription

        private string GetBaseEntity(DataTable dt) {
            string entity = string.Empty;
            var columnList = dt.AsEnumerable().Select(p => p["TableColumn"].ToString()).ToList();

            bool id = columnList.Where(p => p == "Id" || p == "id").Any();
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
