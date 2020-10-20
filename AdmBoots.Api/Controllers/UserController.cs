using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdmBoots.Api.Authorization;
using AdmBoots.Application.Users;
using AdmBoots.Application.Users.Dto;
using AdmBoots.Infrastructure.Domain;
using AdmBoots.Infrastructure.Framework.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdmBoots.Api.Controllers {

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    [Authorize]
    public class UserController : ControllerBase {
        private readonly IUserService _userService;

        public UserController(IUserService userService) {
            _userService = userService;
        }

        [HttpGet]
        [AdmAuthorizeFilter("User:Query")]
        public IActionResult GetUserList([FromQuery]GetUserInput input) {
            var users = _userService.GetUserList(input);
            return Ok(ResponseBody.From(users));
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody]AddOrUpdateUserInput input) {
            await _userService.AddOrUpdateUser(null, input);
            return Ok(ResponseBody.From("保存成功"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody]AddOrUpdateUserInput input) {
            await _userService.AddOrUpdateUser(id, input);
            return Ok(ResponseBody.From("修改成功"));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int[] ids) {
            await _userService.DeleteUser(ids);
            return Ok(ResponseBody.From("删除成功"));
        }

        [HttpPut("{id}/pwd")]
        public async Task<IActionResult> ResetPassword(int id) {
            await _userService.ResetPassword(id);
            return Ok(ResponseBody.From("密码已重置为 123456"));
        }

        [HttpPut("modifyPwd")]
        public async Task<IActionResult> ModifyPassword(ModifyPasswordInput input) {
            await _userService.ModifyPassword(input);
            return Ok(ResponseBody.From("密码修改成功"));
        }

        [HttpGet("permission")]
        public async Task<IActionResult> GetPermissions() {
            var permissions = await _userService.GetPermissions();
            return Ok(ResponseBody.From(permissions));
        }
    }
}
