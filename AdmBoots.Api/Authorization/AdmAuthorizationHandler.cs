using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AdmBoots.Api.Authorization;
using AdmBoots.Application.Roles;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AdmBoots.Infrastructure.Authorization {

    public class AdmAuthorizationHandler : AuthorizationHandler<AdmPolicyRequirement> {
        private readonly IAuthenticationSchemeProvider _schemes;
        private readonly IHttpContextAccessor _accessor;
        private readonly IRoleService _roleService;

        public AdmAuthorizationHandler(IAuthenticationSchemeProvider schemes, IHttpContextAccessor accessor, IRoleService roleService) {
            _schemes = schemes;
            _accessor = accessor;
            _roleService = roleService;
        }

        /// <summary>
        /// 重载异步处理程序
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdmPolicyRequirement requirement) {
            var httpContext = _accessor.HttpContext;
            var routContext = (context.Resource as RouteEndpoint);
            var descriptor = routContext.Metadata.OfType<ControllerActionDescriptor>().FirstOrDefault();
            var currentURI = string.Empty;
            //如果有自定义资源标识，取自定义的标识。没有自定义的，取默认ControllerName:ActionName
            var admAuthorizeFilterAttr = GetAdmAuthorizeFilterAttributeOrNull(descriptor.MethodInfo);
            if (admAuthorizeFilterAttr == null || string.IsNullOrEmpty(admAuthorizeFilterAttr.FilterName)) {
                if (descriptor != null) {
                    currentURI = $"{descriptor.ControllerName}:{descriptor.ActionName}";
                }
            } else {
                currentURI = admAuthorizeFilterAttr.FilterName;
            }
            //判断请求是否停止
            var handlers = httpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            foreach (var scheme in await _schemes.GetRequestHandlerSchemesAsync()) {
                if (await handlers.GetHandlerAsync(httpContext, scheme.Name) is IAuthenticationRequestHandler handler
                    && await handler.HandleRequestAsync()) {
                    context.Fail();
                    return;
                }
            }

            var defaultAuthenticate = await _schemes.GetDefaultAuthenticateSchemeAsync();
            if (defaultAuthenticate != null) {
                var result = await httpContext.AuthenticateAsync(defaultAuthenticate.Name);
                //result?.Principal不为空即登录成功
                if (result?.Principal != null) {
                    httpContext.User = result.Principal;
                    // 获取当前用户的角色信息
                    var currentUserRoles = (from item in httpContext.User.Claims
                                            where item.Type == requirement.ClaimType
                                            select Convert.ToInt32(item.Value)).ToList();
                    // 获取权限列表（role-uri）
                    var roleUris = _roleService.GetRoleUriMaps();
                    var permisssionRoles = roleUris.Where(ru => currentUserRoles.Contains(ru.RoleId));
                    if (!permisssionRoles.Any(pr => currentURI == pr.Uri)) {
                        context.Fail();
                        return;
                    }

                    //判断过期时间
                    //这里仅仅是最坏验证原则，你可以不要这个if else的判断，因为我们使用的官方验证，Token过期后上边的result?.Principal 就为 null 了，进不到这里了，因此这里其实可以不用验证过期时间，只是做最后严谨判断
                    var expirationTime = httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value;
                    if (!string.IsNullOrEmpty(expirationTime) && DateTime.Parse(expirationTime) >= DateTime.Now) {
                        context.Succeed(requirement);
                        return;
                    } else {
                        context.Fail();
                        return;
                    }
                } else {
                    context.Fail();
                    return;
                }
            }

            context.Succeed(requirement);
        }

        private AdmAuthorizeFilterAttribute GetAdmAuthorizeFilterAttributeOrNull(MethodInfo methodInfo) {
            var attrs = methodInfo.GetCustomAttributes(true).OfType<AdmAuthorizeFilterAttribute>().ToArray();
            if (attrs.Length > 0) {
                return attrs[0];
            }

            attrs = methodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(true).OfType<AdmAuthorizeFilterAttribute>().ToArray();
            return attrs.Length > 0 ? attrs[0] : null;
        }
    }
}
