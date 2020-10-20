using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdmBoots.Infrastructure;
using AdmBoots.Infrastructure.Extensions;
using AdmBoots.Infrastructure.Framework.Abstractions;
using AdmBoots.Infrastructure.Helper;
using AdmBoots.Infrastructure.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using StackExchange.Profiling;

namespace AdmBoots.Api.Controllers {

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/charts")]
    //[Authorize]
    public class ChartController : ControllerBase {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IHttpContextAccessor _accessor;

        public ChartController(IHubContext<ChatHub> hubContext, IHttpContextAccessor accessor) {
            _hubContext = hubContext;
            _accessor = accessor;
        }

        [HttpGet]
        public async Task<IActionResult> GetCounts() {
            var num = new RandomNum {
                QQ = new Random().Next(10, 100),
                WeChat = new Random().Next(10, 100),
                Skype = new Random().Next(10, 100),
                Github = new Random().Next(10, 100),
            };
            if (AdmBootsApp.Configuration["Startup:SignalR"].ObjToBool()) {
                await _hubContext.Clients.All.SendAsync("getCount", num);
                string.Format("SignalR：method：{0}  arg1：{1} 时间：{2}", "getCount", JsonConvert.SerializeObject(num), DateTime.Now).WriteSuccessLine();
            }

            return Ok(ResponseBody.From(num, "获取成功"));
        }

        [HttpGet("GetHtmlString")]
        public IActionResult GetHtmlString() {
            var htmlString = MiniProfiler.Current.RenderIncludes(_accessor.HttpContext);
            return Ok(htmlString.Value);
        }
    }

    public class RandomNum {
        public int QQ { get; set; }

        public int WeChat { get; set; }

        public int Skype { get; set; }

        public int Github { get; set; }
    }
}
