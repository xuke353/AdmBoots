using System.Threading.Tasks;
using Adm.Boot.Api.Authorization;
using Adm.Boot.Application.Users;
using Adm.Boot.Application.Users.Dto;
using Adm.Boot.Infrastructure.Authorization;
using Adm.Boot.Infrastructure.Framework.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Adm.Boot.Api.Controllers {
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    [AllowAnonymous]
    public class LoginController {
        private readonly IUserService _userService;
        private readonly AdmPolicyRequirement _requirement;
        private readonly IDistributedCache _cache;

        public LoginController(IUserService userService, AdmPolicyRequirement requirement, IDistributedCache cache) {
            _userService = userService;
            _requirement = requirement;
            _cache = cache;
        }

        [HttpPost("login")]
        public async Task<object> Login([FromBody]LoginInput input) {
            if (string.IsNullOrWhiteSpace(input.UserName)
                || string.IsNullOrWhiteSpace(input.Password)) {
                return ResponseBody.Bad("用户名或密码不能为空");
            }
            var user = await _userService.LonginAsync(input);
            if (user != null) {
                return AuthenticateResult.Get(user, _requirement, _cache);
            }
            return ResponseBody.Bad("用户名或密码错误");
        }

        /// <summary>
        /// 请求刷新Token（以旧换新）
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("refresh-token/{token}")]
        public async Task<object> RefreshToken(string token) {
            if (string.IsNullOrEmpty(token)) {
                return ResponseBody.Bad("令牌无效，请重新登陆");
            }
            var uid = JwtToken.ReadJwtToken<int>(token);

            var user = await _userService.GetLoginUserAsync(uid);
            if (user != null) {
                return AuthenticateResult.Get(user, _requirement, _cache);
            }
            return ResponseBody.Bad("认证失败");
        }
    }
}
