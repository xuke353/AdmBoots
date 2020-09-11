using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdmBoots.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace AdmBoots.Api.Extensions {

    /// <summary>
    /// 健康检查
    /// </summary>
    public static class HealthChecksSetup {

        public static void AddHealthChecksSetup(this IServiceCollection services) {
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
            }).AddInMemoryStorage();
        }
    }
}
