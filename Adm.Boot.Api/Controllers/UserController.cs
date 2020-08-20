using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adm.Boot.Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace Adm.Boot.Api.Controllers {

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    public class UserController : ControllerBase {
        private readonly IUserService _userService;

        public UserController(IUserService userService) {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetUserList() {
            var users = _userService.GetUserList();
            return Ok(users);
        }

        [HttpPost]
        public IActionResult AddUser() {
            _userService.AddUser();
            return Ok("插入成功");
        }

        [HttpPost("async")]
        public async Task<IActionResult> AddUserAsync() {
            await _userService.AddUserAsync();
            return Ok("插入成功");
        }
    }
}
