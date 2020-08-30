using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdmBoots.Application.Users;
using AdmBoots.Application.Users.Dto;
using AdmBoots.Infrastructure.Framework.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace AdmBoots.Api.Controllers {

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    public class UserController : ControllerBase {
        private readonly IUserService _userService;

        public UserController(IUserService userService) {
            _userService = userService;
        }

        [HttpGet("getList")]
        public IActionResult GetUserList([FromQuery]GetUserInput input) {
            var users = _userService.GetUserList(input);
            return Ok(ResponseBody.From(users));
        }
    }
}
