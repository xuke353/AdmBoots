using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AdmBoots.Infrastructure.Auditing {

    public class HttpContextClientInfoProvider : IClientInfoProvider {
        public string BrowserInfo => GetBrowserInfo();

        public string ClientIpAddress => GetClientIpAddress();

        public string ComputerName => GetComputerName();

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpContext _httpContext;
        private readonly ILogger<HttpContextClientInfoProvider> _logger;

        public HttpContextClientInfoProvider(IHttpContextAccessor httpContextAccessor, ILogger<HttpContextClientInfoProvider> logger) {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public string GetBrowserInfo() {
            var httpContext = _httpContextAccessor.HttpContext ?? _httpContext;
            return httpContext?.Request?.Headers?["User-Agent"];
        }

        public string GetClientIpAddress() {
            try {
                var httpContext = _httpContextAccessor.HttpContext ?? _httpContext;
                return httpContext?.Connection?.RemoteIpAddress?.ToString();
            } catch (Exception ex) {
                _logger.LogWarning(ex.ToString());
            }

            return null;
        }

        public string GetComputerName() {
            return null; //TODO: Implement!
        }
    }
}
