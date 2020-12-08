using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AdmBoots.Infrastructure.Authorization;
using AdmBoots.Infrastructure.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AdmBoots.Api.Extensions {

    public static class AuthorizationSetup {

        public static void AddAuthorizationSetup(this IServiceCollection services, IConfiguration configuration) {
            if (services == null) throw new ArgumentNullException(nameof(services));

            //复杂的策略授权
            services.AddAuthorization(options => {
                options.AddPolicy(AdmConsts.POLICY,
                         policy => policy.Requirements.Add(new AdmPolicyRequirement(ClaimTypes.Role)));
            });

            //官方JWT认证
            //开启Bearer认证
            services.AddAuthentication(o => {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             // 添加JwtBearer服务
             .AddJwtBearer(o => {
                 //令牌验证参数
                 o.TokenValidationParameters = new TokenValidationParameters {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = TokenAuthConfiguration.IssuerSigningKey,
                     ValidateIssuer = true,
                     ValidIssuer = TokenAuthConfiguration.Issuer,//发行人
                     ValidateAudience = true,
                     ValidAudience = TokenAuthConfiguration.Audience,//订阅人
                     ValidateLifetime = true,
                     ClockSkew = TimeSpan.Zero,
                     RequireExpirationTime = true,
                 };
                 o.Events = new JwtBearerEvents {
                     OnAuthenticationFailed = context => {
                         // 如果过期，则把<是否过期>添加到，返回头信息中
                         if (context.Exception.GetType() == typeof(SecurityTokenExpiredException)) {
                             context.Response.Headers.Add("Token-Expired", "true");
                         }
                         return Task.CompletedTask;
                     },
                     ////token白名单  一个用户只有一个token有效
                     //OnTokenValidated = context => {
                     //    var token = ((JwtSecurityToken)context.SecurityToken).RawData;
                     //    var uid = JwtToken.ReadJwtToken<int>(token);
                     //    var cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
                     //    if (cache.GetString(uid.ToString()) != token) {
                     //        context.Fail("token不在白名单中");//返回401
                     //    }
                     //    return Task.CompletedTask;
                     //},
                     //对连接到集线器的用户进行身份验证 SignalR
                     //https://docs.microsoft.com/zh-cn/aspnet/core/signalr/authn-and-authz?view=aspnetcore-3.1
                     OnMessageReceived = context => {
                         if (!context.HttpContext.Request.Path.HasValue) {
                             return Task.CompletedTask;
                         }
                         var accessToken = context.Request.Query["access_token"];
                         //判断是Signalr的路径
                         var path = context.HttpContext.Request.Path;
                         if (!string.IsNullOrEmpty(accessToken) &&
                             (path.StartsWithSegments("/api/chatHub"))) {
                             context.Token = accessToken;
                         }
                         return Task.CompletedTask;
                     }
                 };
             });
            // 注入权限处理器
            services.AddScoped<IAuthorizationHandler, AdmAuthorizationHandler>();
        }
    }
}
