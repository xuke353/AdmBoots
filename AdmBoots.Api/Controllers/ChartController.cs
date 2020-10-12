using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdmBoots.Infrastructure.Framework.Abstractions;
using AdmBoots.Infrastructure.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AdmBoots.Api.Controllers {

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/charts")]
    //[Authorize]
    public class ChartController : ControllerBase {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChartController(IHubContext<ChatHub> hubContext) {
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetCounts() {
            var num = new RandomNum {
                QQ = new Random().Next(10, 100),
                WeChat = new Random().Next(10, 100),
                Skype = new Random().Next(10, 100),
                Github = new Random().Next(10, 100),
            };
            await _hubContext.Clients.All.SendAsync("getCount", num);
            return Ok(ResponseBody.From(num, "获取成功"));
        }
    }

    public class RandomNum {
        public int QQ { get; set; }

        public int WeChat { get; set; }

        public int Skype { get; set; }

        public int Github { get; set; }
    }
}
