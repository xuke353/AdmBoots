
using AdmBoots.Application.Roles;
using AdmBoots.Application.Roles.Dto;
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
    [Route("api/v{version:apiVersion}/roles")]
    //[Authorize(AdmConsts.Policy)]
    public class RoleController : ControllerBase {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService) {
            _roleService = roleService;
        }

        [HttpGet]
        public IActionResult GetUserList([FromQuery]GetRoleInput input) {
            var users = _roleService.GetRoleList(input);
            return Ok(ResponseBody.From(users));
        }
    }
}
