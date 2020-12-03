using AdmBoots.Infrastructure;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdmBoots.Api.Extensions {

    /// <summary>
    /// Swagger服务
    /// </summary>
    public static class SwaggerSetup {

        public static void AddSwaggerSetup(this IServiceCollection services, IConfiguration configuration) {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var basePath = AppContext.BaseDirectory;
            var apiName = configuration["Startup:ApiName"];
            services.AddSwaggerGen(c => {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions) {
                    c.SwaggerDoc(description.GroupName, new OpenApiInfo {
                        Version = description.GroupName,
                        Title = $"{apiName} For .NET Core 3.1",
                        Contact = new OpenApiContact { Name = "健康检查", Url = new Uri("http://localhost:8082/healthchecks-ui") },
                    });
                    c.OrderActionsBy(o => o.RelativePath);
                }

                c.IncludeXmlComments(Path.Combine(basePath, "AdmBoots.Api.xml"), true);//true controller的注释
                c.IncludeXmlComments(Path.Combine(basePath, "AdmBoots.Application.xml"));

                // 开启加权小锁
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                //启用oauth2安全授权访问api接口
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                //开启oauth2安全描述
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                });
            });
            services.AddVersionedApiExplorer(options => {
                options.GroupNameFormat = "'v'VVV";

                // 注意: 只有在通过url段进行版本控制时，才需要此选项。替代格式还可以用于控制路由模板中API版本的格式。
                options.SubstituteApiVersionInUrl = true;
            });
        }
    }
}
