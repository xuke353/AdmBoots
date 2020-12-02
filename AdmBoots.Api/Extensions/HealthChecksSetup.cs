using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdmBoots.Data.EntityFrameworkCore;
using AdmBoots.Infrastructure;
using AdmBoots.Infrastructure.Config;
using AdmBoots.Infrastructure.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

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
                  .AddCheck("self", () => HealthCheckResult.Healthy())
                 .AddDbContextCheck<AdmDbContext>(tags: new[] { "services" })
                 //检查数据库连接
                 .AddMySql(DatabaseConfig.ConnectionString);
                //检查Rdis
                //.AddRedis(AdmApp.Configuration["Redis:Configuration"]);
                services.AddHealthChecksUI(setupSettings: setup => {
                    setup.AddHealthCheckEndpoint("endpoint1", "/healthz");
                    setup.AddHealthCheckEndpoint("endpoint2", "/health-process");
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
                // 当前应用是否存活的检查
                endpoints.MapHealthChecks("/health-process", new HealthCheckOptions {
                    Predicate = r => r.Name == "self",
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecksUI();
            }
        }
    }
}
