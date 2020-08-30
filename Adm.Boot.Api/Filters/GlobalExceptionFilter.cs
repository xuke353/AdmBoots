using AdmBoots.Infrastructure.CustomExceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdmBoots.Api.Filters {

    /// <summary>
    /// 全局异常错误日志
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(IWebHostEnvironment env, ILogger<GlobalExceptionFilter> logger) {
            _env = env;
            _logger = logger;
        }

        public void OnException(ExceptionContext context) {
            var result = new ContentResult { ContentType = "text/json;charset=utf-8;" };

            if (context.Exception.GetType() == typeof(BusinessException)) {
                result.StatusCode = 200; //业务异常，统一返回200 并返回{message="",  status = false}结构
                context.Result = new JsonResult(new { context.Exception.Message, status = false });
            } else {
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.Content = _env.IsDevelopment() ? context.Exception.Message : "服务器发生了意外的内部错误";
                context.Result = result;
            }
            //记录到日志
            var detailMessage = WriteLog(context.Exception);
            _logger.LogError(detailMessage);
            //异常已处理
            context.ExceptionHandled = true;
        }

        /// <summary>
        /// 自定义返回格式
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private string WriteLog(Exception ex) {
            return string.Format("【异常信息】：{0} \r\n【异常类型】：{1} \r\n【堆栈调用】：{2}", new object[] { ex.Message,
                ex.GetType().Name, ex.StackTrace });
        }
    }
}
