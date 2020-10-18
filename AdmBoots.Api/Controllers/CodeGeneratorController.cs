using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AdmBoots.Infrastructure.CodeGenerator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

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

        public CodeGeneratorController(IWebHostEnvironment env) {
            _env = env;
        }
        [HttpPost]
        public IActionResult CodeGenerate([FromQuery]CodeGeneratorInput input) {
            var template = new SingleTableTemplate();
            var fileConfig = template.GetFileConfig(_env.ContentRootPath, input);
            var result = new Dictionary<string, string>();
            //if (input.WriteCotroller) {
            //    DataTable dt = new DataTable();//DataTableHelper.ListToDataTable(objTable.Data);
            //    var codeEntity = template.BuildEntity(fileConfig, dt);
            //    result.Add("实体类", fileConfig.OutputEntity);
            //}
            result.Add("实体类", $"{fileConfig.EntityName} -> {fileConfig.OutputEntity}");
            result.Add("服务类", $"{fileConfig.ServiceName} -> {fileConfig.OutputService}");
            return Ok(new { Message = "生成成功", Result = result });
        }
    }

}
