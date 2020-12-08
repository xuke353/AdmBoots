using System.Threading.Tasks;
using AdmBoots.Application.Roles;
using AdmBoots.Application.Roles.Dto;
using AdmBoots.Infrastructure.Auditing;
using AdmBoots.Infrastructure.Authorization;
using AdmBoots.Infrastructure.Domain;
using AdmBoots.Infrastructure.Framework.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdmBoots.Api.Controllers {

    /// <summary>
    ///
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/roles")]
    [Authorize(AdmConsts.POLICY)]
    [Audited]
    public class RoleController : ControllerBase {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService) {
            _roleService = roleService;
        }

        [HttpGet]
        [AdmAuthorizeFilter("Role:Query")]
        [DisableAuditing]
        public IActionResult GetRoleList([FromQuery] GetRoleInput input) {
            var roles = _roleService.GetRoleList(input);
            return Ok(ResponseBody.From(roles));
        }

        [HttpPost]
        [AdmAuthorizeFilter("Role:Add")]
        public async Task<IActionResult> AddRole([FromBody] AddOrUpdateRoleInput input) {
            await _roleService.AddOrUpdateRole(null, input);
            return Ok(ResponseBody.From("保存成功"));
        }

        [HttpPut("{id}")]
        [AdmAuthorizeFilter("Role:Update")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] AddOrUpdateRoleInput input) {
            await _roleService.AddOrUpdateRole(id, input);
            return Ok(ResponseBody.From("修改成功"));
        }

        [HttpDelete]
        [AdmAuthorizeFilter("Role:Delete")]
        public async Task<IActionResult> DeleteRole(int[] ids) {
            await _roleService.DeleteRole(ids);
            return Ok(ResponseBody.From("删除成功"));
        }

        [HttpGet("transferRoles")]
        [AllowAnonymous]
        public IActionResult GetTransferRoles() {
            var roles = _roleService.GetTransferRoles();
            return Ok(ResponseBody.From(roles));
        }

        [HttpPut("updateRoleMenu")]
        public async Task<IActionResult> UpdateRoleMenu([FromBody] UpdateRoleMenuInput input) {
            await _roleService.UpdateRoleMenu(input);
            return Ok(new { Status = true, Message = "更新成功" });
        }
    }
}
