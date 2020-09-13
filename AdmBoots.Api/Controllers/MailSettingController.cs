
using AdmBoots.Application.MailSettings;
using AdmBoots.Application.MailSettings.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AdmBoots.Infrastructure.Domain;
using AdmBoots.Infrastructure.Framework.Abstractions;

namespace AdmBoots.Api.Controllers {
    /// <summary>
    ///
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/mailSettings")]
    [Authorize]
    public class MailSettingController : ControllerBase {
        private readonly IMailSettingService _mailSettingService;
        public MailSettingController(IMailSettingService mailSettingService) {
            _mailSettingService = mailSettingService;
        }

        [HttpGet]
        public IActionResult GetMailSettingList() {
            var result = _mailSettingService.GetMailSetting();
            return Ok(ResponseBody.From(result));
        }
        [HttpPost]
        public IActionResult AddMailSetting([FromBody]AddMailSettingInput input) {
            _mailSettingService.AddMailSetting(input);
            return Ok(ResponseBody.From("保存成功"));
        }
    }
}
