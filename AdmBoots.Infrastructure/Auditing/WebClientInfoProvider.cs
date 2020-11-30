using System;
using System.Net;
using System.Net.Sockets;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AdmBoots.Infrastructure.Auditing {
    public class WebClientInfoProvider : IClientInfoProvider {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public string BrowserInfo => GetBrowserInfo();

        public string ClientIpAddress => GetClientIpAddress();

        public string ComputerName => GetComputerName();

        public ILogger<WebClientInfoProvider> Logger { get; private set; }
        public HttpContext HttpContext { get; private set; }
        /// <summary>
        /// Creates a new <see cref="WebClientInfoProvider"/>.
        /// </summary>
        public WebClientInfoProvider(ILogger<WebClientInfoProvider> logger, IHttpContextAccessor httpContextAccessor) {
            Logger = logger;
            HttpContext = httpContextAccessor.HttpContext;
        }

        protected virtual string GetBrowserInfo() {
            var httpContext = HttpContext;
            if (httpContext?.Request.Browser == null) {
                return null;
            }

            return httpContext.Request.Browser.Browser + " / " +
                   httpContext.Request.Browser.Version + " / " +
                   httpContext.Request.Browser.Platform;
        }

        protected virtual string GetClientIpAddress() {
            var httpContext = HttpContext.Current;
            if (httpContext?.Request.ServerVariables == null) {
                return null;
            }

            var clientIp = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
                           httpContext.Request.ServerVariables["REMOTE_ADDR"];

            try {
                foreach (var hostAddress in Dns.GetHostAddresses(clientIp)) {
                    if (hostAddress.AddressFamily == AddressFamily.InterNetwork) {
                        return hostAddress.ToString();
                    }
                }

                foreach (var hostAddress in Dns.GetHostAddresses(Dns.GetHostName())) {
                    if (hostAddress.AddressFamily == AddressFamily.InterNetwork) {
                        return hostAddress.ToString();
                    }
                }
            } catch (Exception ex) {
                Logger.LogDebug(ex.ToString());
            }

            return clientIp;
        }

        protected virtual string GetComputerName() {
            var httpContext = HttpContext.Current;
            if (httpContext == null || !httpContext.Request.IsLocal) {
                return null;
            }

            try {
                var clientIp = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
                               httpContext.Request.ServerVariables["REMOTE_ADDR"];
                return Dns.GetHostEntry(IPAddress.Parse(clientIp)).HostName;
            } catch {
                return null;
            }
        }
    }
}
