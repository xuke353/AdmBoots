using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdmBoots.Infrastructure;
using AdmBoots.Infrastructure.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AdmBoots.Api.Extensions {

    /// <summary>
    /// 健康检查
    /// </summary>
    public static class HealthChecksSetup {

        public static void AddHealthChecksSetup(this IServiceCollection services) {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (AdmBootsApp.Configuration["Startup:HealthChecks"].ObjToBool()) {
                //https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks
                services.AddHealthChecks()
                 //检查数据库连接
                 .AddMySql(AdmBootsApp.Configuration["Database:ConnectionString"]);
                //检查Rdis
                //.AddRedis(AdmApp.Configuration["Redis:Configuration"]);
                services.AddHealthChecksUI(setupSettings: setup => {
                    setup.AddHealthCheckEndpoint("endpoint1", "/healthz");
                    //运行状况检查间隔秒数
                    setup.SetMinimumSecondsBetweenFailureNotifications(60);
                    //故障通知之间的最短秒数
                    setup.SetEvaluationTimeInSeconds(10);
                }).AddInMemoryStorage();//可以使用数据库，避免内存占用，具体参考官网
            }
        }

        public static void UseHealthChecks(this IEndpointRouteBuilder endpoints) {
            if (AdmBootsApp.Configuration["Startup:HealthChecks"].ObjToBool()) {
                //健康检查 输出描述信息
                endpoints.MapHealthChecks("/healthz", new HealthCheckOptions {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecksUI();
            }
        }
    }
}
