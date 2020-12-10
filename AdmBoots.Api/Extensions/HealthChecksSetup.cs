using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdmBoots.Data.EntityFrameworkCore;
using AdmBoots.Infrastructure;
using AdmBoots.Infrastructure.Config;
using AdmBoots.Infrastructure.Extensions;
using AdmBoots.Infrastructure.Ioc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AdmBoots.Api.Extensions {

    /// <summary>
    /// 健康检查
    /// </summary>
    public static class HealthChecksSetup {

        public static void AddHealthChecksSetup(this IServiceCollection services, IConfiguration configuration) {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (configuration.GetValue("Startup:HealthChecks", false)) {
                //https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks
                services.AddHealthChecks()
                 //创建自己的健康检查
                 .AddCheck("存活检查", () => HealthCheckResult.Healthy(), tags: new[] { "app", "self" })
                 .AddDbContextCheck<AdmDbContext>(name: "AdmDbContext", tags: new[] { "app", "services" })
                 .AddProcessAllocatedMemoryHealthCheck(2048, "占用内存是否超过阀值(2G)", tags: new[] { "app", "memory" })
                 .AddMySql(DatabaseConfig.ConnectionString, "mysql", tags: new[] { "db", "mysql" })
                 .AddRedis(configuration["Redis:Configuration"], tags: new[] { "db", "redis" });
                services.AddHealthChecksUI(setupSettings: setup => {
                    setup.AddHealthCheckEndpoint("endpoint1", "/healthz");
                    setup.AddHealthCheckEndpoint("endpoint2", "/health-db");
                    //运行状况检查间隔秒数
                    setup.SetMinimumSecondsBetweenFailureNotifications(60);
                    //故障通知之间的最短秒数
                    setup.SetEvaluationTimeInSeconds(10);
                }).AddInMemoryStorage();//可以使用数据库，避免内存占用，具体参考官网
            }
        }

        public static void UseHealthChecks(this IEndpointRouteBuilder endpoints) {
            var configuration = IocManager.Current.Resolve<IConfiguration>();
            if (configuration.GetValue("Startup:HealthChecks", false)) {
                endpoints.MapHealthChecks("/healthz", new HealthCheckOptions {
                    Predicate = r => r.Tags.Contains("app"),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/health-db", new HealthCheckOptions {
                    Predicate = r => r.Tags.Contains("db"),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecksUI();
            }
        }
    }
}
