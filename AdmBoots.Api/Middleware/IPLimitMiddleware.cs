using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AdmBoots.Api.Middleware {

    /// <summary>
    /// 加入限流后，在被限流的接口调用后，限流拦截器使得跨域策略失效，故重写拦截器中间件，继承IpRateLimitMiddleware 即可
    /// https://www.cnblogs.com/EminemJK/p/12720691.html
    /// </summary>
    public class IPLimitMiddleware : IpRateLimitMiddleware {

        public IPLimitMiddleware(RequestDelegate next, IOptions<IpRateLimitOptions> options, IRateLimitCounterStore counterStore, IIpPolicyStore policyStore, IRateLimitConfiguration config, ILogger<IpRateLimitMiddleware> logger)
            : base(next, options, counterStore, policyStore, config, logger) {
        }

        public override Task ReturnQuotaExceededResponse(HttpContext httpContext, RateLimitRule rule, string retryAfter) {
            httpContext.Response.Headers.Append("Access-Control-Allow-Origin", "*");
            return base.ReturnQuotaExceededResponse(httpContext, rule, retryAfter);
        }
    }
}
