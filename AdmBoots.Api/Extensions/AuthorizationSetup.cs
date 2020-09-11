using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AdmBoots.Api.Authorization;
using AdmBoots.Infrastructure;
using AdmBoots.Infrastructure.Authorization;
using AdmBoots.Infrastructure.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AdmBoots.Api.Extensions {

    public static class AuthorizationSetup {

        public static void AddAuthorizationSetup(this IServiceCollection services) {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var secretKey = AdmBootsApp.Configuration["Authentication:JwtBearer:SecurityKey"];
            var keyByteArray = Encoding.ASCII.GetBytes(secretKey);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var issuer = AdmBootsApp.Configuration["Authentication:JwtBearer:Issuer"];
            var audience = AdmBootsApp.Configuration["Authentication:JwtBearer:Audience"];

            var admPolicyRequirement = new AdmPolicyRequirement(
                                ClaimTypes.Role,//基于角色的授权
                                issuer,//发行人
                                audience,//订阅人
                                new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),//签名凭据
                                expiration: TimeSpan.FromDays(1)//接口的过期时间 总的Token有效时间 = 接口的过期时间 + ClockSkew
                             );

            //复杂的策略授权
            services.AddAuthorization(options => {
                options.AddPolicy(AdmConsts.POLICY,
                         policy => policy.Requirements.Add(admPolicyRequirement));
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
                     IssuerSigningKey = signingKey,
                     ValidateIssuer = true,
                     ValidIssuer = issuer,//发行人
                     ValidateAudience = true,
                     ValidAudience = audience,//订阅人
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
                     //OnTokenValidated = context => {
                     //    var token = ((JwtSecurityToken)context.SecurityToken).RawData;
                     //    var uid = JwtToken.ReadJwtToken<int>(token);
                     //    var cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
                     //    if (cache.GetString(uid.ToString()) != token) {
                     //        context.Fail("token不在白名单中");//返回401
                     //    }
                     //    return Task.CompletedTask;
                     //},
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
            services.AddSingleton(admPolicyRequirement);
        }
    }
}
