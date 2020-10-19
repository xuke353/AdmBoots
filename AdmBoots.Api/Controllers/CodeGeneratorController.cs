using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdmBoots.Domain.IRepositories;
using AdmBoots.Infrastructure;
using AdmBoots.Infrastructure.CodeGenerator;
using AdmBoots.Infrastructure.Config;
using AdmBoots.Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace AdmBoots.Api.Controllers {

    /// <summary>
    /// 代码生成器
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/codeGenerators")]
    [AllowAnonymous]
    public class CodeGeneratorController : ControllerBase {
        private readonly IWebHostEnvironment _env;
        private readonly ISqlExecuterRepository _sqlExecuterRepository;

        public CodeGeneratorController(IWebHostEnvironment env, ISqlExecuterRepository sqlExecuterRepository) {
            _env = env;
            _sqlExecuterRepository = sqlExecuterRepository;
        }

        [HttpPost]
        public IActionResult CodeGenerate([FromQuery]CodeGeneratorInput input) {
            var template = new SingleTableTemplate();
            var fileConfig = template.GetFileConfig(_env.ContentRootPath, input);
            var result = new Dictionary<string, string>();
            if (input.WriteModel) {
                StringBuilder strSql = new StringBuilder();
                strSql.Append(@"SELECT COLUMN_NAME TableColumn,
		                           DATA_TYPE Datatype,
		                           (CASE COLUMN_KEY WHEN 'PRI' THEN COLUMN_NAME ELSE '' END) TableIdentity,
		                           REPLACE(REPLACE(SUBSTRING(COLUMN_TYPE,LOCATE('(',COLUMN_TYPE)),'(',''),')','') FieldLength,
	                               (CASE IS_NULLABLE WHEN 'NO' THEN 'N' ELSE 'Y' END) IsNullable,
                                   IFNULL(COLUMN_DEFAULT,'') FieldDefault,
                                   COLUMN_COMMENT Remark
                             FROM information_schema.columns WHERE table_schema='" + GetDatabase() + "' AND table_name='" + fileConfig.TableName + "'");
                var fieldList = _sqlExecuterRepository.SqlQuery<TableFieldInfo>(strSql.ToString()).ToList();
                if (fieldList.Count < 1) {
                    result.Add("实体类", $"生成失败！未在{GetDatabase()}中找到表{fileConfig.TableName}");
                } else {
                    DataTable dt = DataTableHelper.ListToDataTable(fieldList);
                    var codeEntity = template.BuildEntity(fileConfig, dt);
                    result.Add("实体类", "生成成功！");
                    FileHelper.OutCode2File(fileConfig.OutputEntity, fileConfig.EntityName, codeEntity);
                }
            }
            if (input.WriteCotroller) {
                var codeCotroller = template.BuildController(fileConfig);
                result.Add("控制类", "生成成功！");
                FileHelper.OutCode2File(fileConfig.OutputController, fileConfig.ControllerName, codeCotroller);
            }
            if (input.WriteService) {
                var codeService = template.BuildService(fileConfig);
                result.Add("服务类", "生成成功！");
                FileHelper.OutCode2File(fileConfig.OutputService, fileConfig.ServiceName, codeService);
                var codeIService = template.BuildIService(fileConfig);
                result.Add("服务接口", "生成成功！");
                FileHelper.OutCode2File(fileConfig.OutputService, fileConfig.IServiceName, codeIService);
            }
            if (input.WriteDto) {
                var codeDtoGetInputName = template.BuildDto(fileConfig, $"{fileConfig.DtoGetInputName} : PageRequest");
                FileHelper.OutCode2File(fileConfig.OutputDto, fileConfig.DtoGetInputName, codeDtoGetInputName);
                var codeDtoGetOutputName = template.BuildDto(fileConfig, fileConfig.DtoGetOutputName);
                FileHelper.OutCode2File(fileConfig.OutputDto, fileConfig.DtoGetOutputName, codeDtoGetOutputName);
                var codeDtoUpdateInputName = template.BuildDto(fileConfig, fileConfig.DtoUpdateInputName);
                FileHelper.OutCode2File(fileConfig.OutputDto, fileConfig.DtoUpdateInputName, codeDtoUpdateInputName);
                result.Add("DTO", "生成成功！");
            }
            return Ok(result);
        }

        private string GetDatabase() {
            var connectionString = DatabaseConfig.ConnectionString;
            var prefix = "Database=";
            var subfix = ";";
            int inl = connectionString.IndexOf(prefix);
            if (inl == -1) {
                return null;
            }
            inl += prefix.Length;
            int inl2 = connectionString.IndexOf(subfix, inl);
            string database = connectionString.Substring(inl, inl2 - inl);
            return database;
        }
    }
}
