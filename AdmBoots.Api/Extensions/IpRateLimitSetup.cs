using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdmBoots.Api.Extensions {

    public static class IpRateLimitSetup {

        public static void AddIpRateLimitSetup(this IServiceCollection services, IConfiguration configuration) {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (!configuration.GetValue("Startup:IpRateLimit", false)) {
                return;
            }
            //加载配置
            //services.AddOptions();
            //从appsettings.json获取相应配置
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

            //注入计数器和规则存储
            services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();

            //配置（计数器密钥生成器）
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}
