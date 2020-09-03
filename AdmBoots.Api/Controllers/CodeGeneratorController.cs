using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdmBoots.Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace AdmBoots.Api.Controllers {

    /// <summary>
    /// 代码生成器
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [AllowAnonymous]
    public class CodeGeneratorController : ControllerBase {
        private readonly IWebHostEnvironment _env;

        public CodeGeneratorController(IWebHostEnvironment env) {
            _env = env;
        }

        /// <summary>
        /// 获取 框架 文件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetFrameFiles([FromQuery]CodeGeneratorInput input) {
            if (input.WriteServices)
                CreateServiceFiles(input.ModelName);
            if (input.WriteIServices)
                CreateIServiceFiles(input.ModelName);
            if (input.WriteCotrollers)
                CreateControllerFiles(input.ModelName);
            if (input.WriteDtos)
                CreateDtoFiles(input.ModelName);
            return Ok(new { Status = true, Mesaage = "文件生成成功" });
        }

        private void CreateControllerFiles(string names) {
            if (string.IsNullOrWhiteSpace(names))
                throw new ArgumentNullException("请输入名称，多个文件名称用,分割");
            //表名
            var tbNames = names.Split(",");
            foreach (var name in tbNames) {
                var className = name.Substring(0, 1).ToUpper() + name.Substring(1);
                //输出路径
                var outputPath = Path.Combine(_env.ContentRootPath, "Controllers");
                //命名空间
                var nameSpace = $"AdmBoots.Api.Controllers";
                //文件名
                var fileName = $"{className}Controller";
                //小写类名
                var lowerClassName = name.Substring(0, 1).ToLower() + name.Substring(1);
                var route = "\"api/v{version:apiVersion}/" + $"{lowerClassName}s\"";
                var addOkMsg = "\"保存成功\"";
                var updateOkMsg = "\"修改成功\"";
                var deleteOkMsg = "\"删除成功\"";
                var urlId = "\"{id}\"";
                var version = "\"1.0\"";
                var content = @$"
using AdmBoots.Application.{className}s;
using AdmBoots.Application.{className}s.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AdmBoots.Infrastructure.Domain;
using AdmBoots.Infrastructure.Framework.Abstractions;

namespace {nameSpace}" + @"
{
	/// <summary>
	///
	/// </summary>
    [ApiController]" + @$"
    [ApiVersion({version})]" + @$"
    [Route({route})]
    [Authorize(AdmConsts.POLICY)]
    public class " + $"{fileName} : ControllerBase" + @"
	{
        private readonly " + @$"I{className}Service _{lowerClassName}Service;
        public " + @$"{fileName}(I{className}Service {lowerClassName}Service)" + @"
        {
            " + @$"_{lowerClassName}Service = {lowerClassName}Service;" + @"
        }

        [HttpGet]
        " + $"public IActionResult Get{className}List([FromQuery]Get{className}Input input)" + "{" + @"
        " + $"            var result = _{lowerClassName}Service.Get{className}List(input);" + @"
                    return Ok(ResponseBody.From(result));
                }
        [HttpPost]
        " + $"public async Task<IActionResult> Add{className}([FromBody]AddOrUpdate{className}Input input)" + "{" + @$"
            await _{lowerClassName}Service.AddOrUpdate{className}(null, input);
            return Ok(ResponseBody.From({addOkMsg}));
        " + "}" + @$"

        [HttpPut({urlId})]
        public async Task<IActionResult> Update{className}(int id, [FromBody]AddOrUpdate{className}Input input) " + "{" + @$"
            await _{lowerClassName}Service.AddOrUpdate{className}(id, input);
            return Ok(ResponseBody.From({updateOkMsg}));
         " + "}" + @$"

        [HttpDelete]
        public async Task<IActionResult> Delete{className}(int[] ids) " + "{" + @$"
            await _{lowerClassName}Service.Delete{className}(ids);
            return Ok(ResponseBody.From({deleteOkMsg}));
        " + "}" + @"
    " + "}" + @"
}";
                FileHelper.OutCode2File(outputPath, fileName, content);
            }
        }

        private void CreateIServiceFiles(string tableNames) {
            if (string.IsNullOrWhiteSpace(tableNames))
                throw new ArgumentNullException("表名必须输入");
            //表名
            string[] tbNames = tableNames.Split(",");
            foreach (var name in tbNames) {
                string className = name.Substring(0, 1).ToUpper() + name.Substring(1);
                var info = new DirectoryInfo(_env.ContentRootPath);
                var path = info.Parent.FullName;
                //输出路径
                string outputPath = Path.Combine(path, @"AdmBoots.Application", $"{className}s");
                //命名空间
                string nameSpace = $"AdmBoots.Application.{className}s";
                //文件名
                string fileName = $"I{className}Service";
                var content = @$"
using AdmBoots.Infrastructure.Framework.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdmBoots.Application.{className}s.Dto;
namespace {nameSpace}" + @"
{
	/// <summary>
	///
	/// </summary>
    public interface " + $"{fileName} :ITransientDependency" + @"
	{
        " + $"Task<Page<Get{className}Output>> Get{className}List(Get{className}Input input);" + @"

        " + $"Task AddOrUpdate{className}(int? id, AddOrUpdate{className}Input input);" + @"

        " + $"Task Delete{className}(int[] ids);" + @"
    }
}
                    ";
                FileHelper.OutCode2File(outputPath, fileName, content);
            }
        }

        private void CreateServiceFiles(string tableNames) {
            if (string.IsNullOrWhiteSpace(tableNames))
                throw new ArgumentNullException("表名必须输入");
            //表名
            string[] tbNames = tableNames.Split(",");
            foreach (var name in tbNames) {
                string className = name.Substring(0, 1).ToUpper() + name.Substring(1);
                var info = new DirectoryInfo(_env.ContentRootPath);
                var path = info.Parent.FullName;
                //输出路径
                string outputPath = Path.Combine(path, "AdmBoots.Application", $"{className}s");
                //命名空间
                string nameSpace = $"AdmBoots.Application.{className}s";
                //文件名
                string fileName = $"{className}Service";
                //小写类名
                string lowerclassName = name.Substring(0, 1).ToLower() + name.Substring(1);
                var content = @$"
using AdmBoots.Application.{className}s.Dto;
using AdmBoots.Domain.IRepositories;
using AdmBoots.Domain.Models;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdmBoots.Infrastructure.Framework.Abstractions;
namespace {nameSpace}" + @"
{
	/// <summary>
	///                                                                                                                                                                                                                                  
	/// </summary>
    public class " + $"{fileName} : AppServiceBase, I{fileName}" + @"
	{
        private readonly " + @$"IRepository<{className}, int> _{lowerclassName}Repository;
        public " + @$"{fileName}(IRepository<{className}, int> {lowerclassName}Repository)" + @"
        {
            " + @$"_{lowerclassName}Repository = {lowerclassName}Repository;" + @"
        }
        " + $"public  Task AddOrUpdate{className}(int? id, AddOrUpdate{className}Input input)" + "{" + @"
            throw new System.NotImplementedException();
        }

        " + $"public Task Delete{className}(int[] ids)" + "{" + @"
            throw new System.NotImplementedException();
        }

        " + $"public  Task<Page<Get{className}Output>> Get{className}List(Get{className}Input input) " + "{" + @"
            throw new System.NotImplementedException();
        }
    }
}
                    ";
                FileHelper.OutCode2File(outputPath, fileName, content);
            }
        }

        private void CreateDtoFiles(string tableNames) {
            if (string.IsNullOrWhiteSpace(tableNames))
                throw new ArgumentNullException("表名必须输入");
            //表名
            string[] tbNames = tableNames.Split(",");
            foreach (var name in tbNames) {
                string className = name.Substring(0, 1).ToUpper() + name.Substring(1);
                var info = new DirectoryInfo(_env.ContentRootPath);
                var path = info.Parent.FullName;
                //输出路径
                string outputPath = Path.Combine(path, @"AdmBoots.Application", @$"{className}s\Dto");
                //命名空间
                string nameSpace = $"AdmBoots.Application.{className}s.Dto";
                //文件名
                string addUpdateInputName = $"AddOrUpdate{className}Input";
                string getInputName = $"Get{className}Input";
                string getOutputName = $"Get{className}Output";

                var content1 = @$"
namespace {nameSpace}" + @"
{
	/// <summary>
	///
	/// </summary>
    public class " + $"{addUpdateInputName} " + @"
	{
    }
}
                    ";
                FileHelper.OutCode2File(outputPath, addUpdateInputName, content1);

                var content2 = @$"
using AdmBoots.Infrastructure.Framework.Abstractions;
namespace {nameSpace}" + @"
{
	/// <summary>
	///
	/// </summary>
    public class " + $"{getInputName} : PageRequest" + @"
	{
    }
}
                    ";
                FileHelper.OutCode2File(outputPath, getInputName, content2);

                var content3 = @$"

namespace {nameSpace}" + @"
{
	/// <summary>
	///
	/// </summary>
    public class " + $"{getOutputName} " + @"
	{
    }
}
                    ";
                FileHelper.OutCode2File(outputPath, getOutputName, content3);
            }
        }
    }

    public class CodeGeneratorInput {

        /// <summary>
        /// Model（,分割）
        /// </summary>
        [Required]
        public string ModelName { get; set; }

        /// <summary>
        /// 输出Cotroller
        /// </summary>
        public bool WriteCotrollers { get; set; } = true;

        /// <summary>
        /// 输出IService
        /// </summary>
        public bool WriteIServices { get; set; } = true;

        /// <summary>
        /// 输出Service
        /// </summary>
        public bool WriteServices { get; set; } = true;

        /// <summary>
        /// 输出Dto
        /// </summary>
        public bool WriteDtos { get; set; } = true;
    }
}
