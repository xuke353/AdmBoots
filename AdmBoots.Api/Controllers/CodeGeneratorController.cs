using System;
using System.Collections.Generic;
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

        [HttpGet]
        public void CreateControllerFiles(string names) {
            if (string.IsNullOrWhiteSpace(names))
                throw new ArgumentNullException("请输入名称，多个文件名称用,分割");
            //表名
            var tbNames = names.Split(",");
            foreach (var name in tbNames) {
                var className = name.Substring(0, 1).ToUpper() + name.Substring(1);
                //输出路径
                var outputPath = Path.Combine(_env.ContentRootPath + @"AdmBoots.Api", "Controllers");
                //命名空间
                var nameSpace = $"AdmBoots.Api.Controllers";
                //文件名
                var fileName = $"{className}Controller";
                //小写类名
                var lowerClassName = name.Substring(0, 1).ToLower() + name.Substring(1);
                var route = "\"api/v{version:apiVersion}/[controller]/[action]\"";
                var version = "\"1.0\"";
                var content = @$"
using AdmBoots.Application.{className}s;
using AdmBoots.Application.{className}s.Dto;
using AdmBoots.Domain.Framework.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AdmBoots.Common;

namespace {nameSpace}" + @"
{	
	/// <summary>
	/// 
	/// </summary>
    [ApiController]" + @$"
    [ApiVersion({version})]" + @$"
    [Route({route})]
    [Authorize(GlobalVars.Permissions)]
    public class " + $"{fileName} : ControllerBase" + @"
	{
        private readonly " + @$"I{className}Service _{lowerClassName}Service;
        public " + @$"{fileName}(I{className}Service {lowerClassName}Service)" + @"
        {
            " + @$"_{lowerClassName}Service = {lowerClassName}Service;" + @"
        }
       
    }
}";
                FileHelper.OutCode2File(outputPath, fileName, content);
            }
        }
    }
}
