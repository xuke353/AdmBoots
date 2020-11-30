using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AdmBoots.Application.Auditings;
using AdmBoots.Application.Auditings.Dto;
using AdmBoots.Infrastructure.Auditing;
using AdmBoots.Infrastructure.Extensions;
using AdmBoots.Infrastructure.Framework.Web;
using AdmBoots.Infrastructure.Ioc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AdmBoots.Api.Filters {
    public class AuditActionFilter : IAsyncActionFilter {
        private readonly IAuditLogService _auditLogService;
        private readonly IAdmSession _admSession;
        private readonly ILoggerFactory _logger;

        public AuditActionFilter(IAuditLogService auditLogService, IAdmSession admSession, ILoggerFactory logger) {
            _auditLogService = auditLogService;
            _admSession = admSession;
            _logger = logger;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
            if (!ShouldSaveAudit(context)) {
                await next();
                return;
            }

            var type = (context.ActionDescriptor as ControllerActionDescriptor).ControllerTypeInfo.AsType();
            var method = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo;
            var arguments = context.ActionArguments;
            var stopwatch = Stopwatch.StartNew();

            var auditInfo = new AuditInfo {
                UserId = _admSession?.UserId,
                ServiceName = type != null
                    ? type.FullName
                    : "",
                MethodName = method.Name,
                Parameters = ConvertArgumentsToJson(arguments),
                ExecutionTime = DateTime.Now
            };

            ActionExecutedContext result = null;
            try {
                result = await next();
                if (result.Exception != null && !result.ExceptionHandled) {
                    auditInfo.Exception = result.Exception;
                }
            } catch (Exception ex) {
                auditInfo.Exception = ex;
                throw;
            } finally {
                stopwatch.Stop();
                auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

                if (result != null) {
                    switch (result.Result) {
                        case ObjectResult objectResult:
                            auditInfo.ReturnValue = JsonConvert.SerializeObject(objectResult.Value);
                            break;

                        case JsonResult jsonResult:
                            auditInfo.ReturnValue = JsonConvert.SerializeObject(jsonResult.Value);
                            break;

                        case ContentResult contentResult:
                            auditInfo.ReturnValue = contentResult.Content;
                            break;
                    }
                }

                await _auditLogService.SaveAsync(auditInfo);
            }
        }

        private static bool ShouldSaveAudit(ActionExecutingContext context, bool defaultValue = false) {
            if (!(context.ActionDescriptor is ControllerActionDescriptor))
                return false;
            var methodInfo = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo;

            if (methodInfo == null) {
                return false;
            }

            if (!methodInfo.IsPublic) {
                return false;
            }

            if (methodInfo.IsDefined(typeof(AuditedAttribute), true)) {
                return true;
            }

            if (methodInfo.IsDefined(typeof(DisableAuditingAttribute), true)) {
                return false;
            }

            var classType = methodInfo.DeclaringType;
            if (classType != null) {
                if (classType.GetTypeInfo().IsDefined(typeof(AuditedAttribute), true)) {
                    return true;
                }

                if (classType.GetTypeInfo().IsDefined(typeof(DisableAuditingAttribute), true)) {
                    return false;
                }
            }
            return defaultValue;
        }

        private string ConvertArgumentsToJson(IDictionary<string, object> arguments) {
            try {
                if (arguments.IsNullOrEmpty()) {
                    return "{}";
                }

                var dictionary = new Dictionary<string, object>();

                foreach (var argument in arguments) {
                    dictionary[argument.Key] = argument.Value;
                }

                return JsonConvert.SerializeObject(dictionary);
            } catch (Exception ex) {
                _logger.CreateLogger<AuditActionFilter>().LogWarning(ex.ToString(), ex);
                return "{}";
            }
        }
    }
}
