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
    [Authorize]
    public class MenuController : ControllerBase {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService) {
            _menuService = menuService;
        }

        [HttpGet("allMenus")]
        public IActionResult GetAllMenus([FromQuery]GetMenuInput input) {
            var result = _menuService.GetAllMenus(input);
            return Ok(ResponseBody.From(result));
        }

        [HttpPost("roleMenus")]
        public IActionResult GetMenusByRole() {
            var result = _menuService.GetMenusByRole();
            return Ok(ResponseBody.From(result));
        }

        [HttpGet("activeMenus")]
        public IActionResult GetActiveMenus() {
            var result = _menuService.GetActiveMenus();
            return Ok(ResponseBody.From(result));
        }

        [HttpGet("cascadeMenus")]
        public IActionResult GetCascadeMenus() {
            var result = _menuService.GetCascadeMenus();
            return Ok(ResponseBody.From(result));
        }

        [HttpPost]
        public IActionResult AddMenu([FromBody]AddOrUpdateMenuInput input) {
            _menuService.AddOrUpdateMenu(null, input);
            return Ok(ResponseBody.From("保存成功"));
        }

        [HttpPut("{id}")]
        public IActionResult UpdateRole(int id, [FromBody]AddOrUpdateMenuInput input) {
            _menuService.AddOrUpdateMenu(id, input);
            return Ok(ResponseBody.From("修改成功"));
        }

        [HttpDelete]
        public IActionResult DeleteMenu(int[] ids) {
            _menuService.DeleteMenu(ids);
            return Ok(ResponseBody.From("删除成功"));
        }
    }
}
