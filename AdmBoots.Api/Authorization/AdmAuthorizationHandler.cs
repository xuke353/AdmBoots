using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AdmBoots.Application.Roles;
using AdmBoots.Infrastructure.Extensions;
using AdmBoots.Infrastructure.Ioc;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AdmBoots.Infrastructure.Authorization {

    public class AdmAuthorizationHandler : AuthorizationHandler<AdmPolicyRequirement> {

        /// <summary>
        /// 重载异步处理程序
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdmPolicyRequirement requirement) {
            var ctx = IocManager.Current.Resolve<IHttpContextAccessor>().HttpContext;
            var ctxUri = GetControllerAction(ctx);
            if (ctx.User.Identity?.IsAuthenticated != true) {
                context.Fail();
                return Task.CompletedTask;
            }
            var claimsIdentity = ctx.User.Identity.CastTo<ClaimsIdentity>();
            // 获取当前用户的角色信息
            var ctxRoleIds = claimsIdentity.FindAll(requirement.ClaimType).Select(claim => claim.Value.ObjToInt());
            var roleService = IocManager.Current.Resolve<IRoleService>();
            // 获取权限列表（role-uri）
            var roleUris = roleService.GetRoleUriMaps();
            var checkPermisson = roleUris.Where(ru => ctxRoleIds.Contains(ru.RoleId))
                .Any(ru => ctxUri.ToLower() == ru.Uri.ToLower());
            if (!checkPermisson) {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        private static string GetControllerAction(HttpContext ctx) {
            var endpoint = ctx.Features.Get<IEndpointFeature>()?.Endpoint;
            var descriptor = endpoint.Metadata.OfType<ControllerActionDescriptor>().FirstOrDefault();
            var controllerAction = string.Empty;
            //如果有自定义资源标识，取自定义的标识。没有自定义的，取默认ControllerName:ActionName
            var admAuthorizeFilterAttr = descriptor?.MethodInfo.GetAttribute<AdmAuthorizeFilterAttribute>();
            if (admAuthorizeFilterAttr == null || string.IsNullOrEmpty(admAuthorizeFilterAttr.FilterName)) {
                if (descriptor != null) {
                    controllerAction = $"{descriptor.ControllerName}:{descriptor.ActionName}";
                }
            } else {
                controllerAction = admAuthorizeFilterAttr.FilterName;
            }
            return controllerAction;
        }
    }
}
