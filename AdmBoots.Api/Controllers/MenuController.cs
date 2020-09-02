
using AdmBoots.Application.Menus;
using AdmBoots.Application.Menus.Dto;
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
    [Route("api/v{version:apiVersion}/menus")]
    [Authorize(AdmConsts.POLICY)]
    public class MenuController : ControllerBase {
        private readonly IMenuService _menuService;
        public MenuController(IMenuService menuService) {
            _menuService = menuService;
        }

        [HttpGet("allMenus")]
        public IActionResult GetAllMenus([FromQuery]GetMenuInput input) {
            var users = _menuService.GetAllMenus(input);
            return Ok(ResponseBody.From(users));
        }
    }
}
