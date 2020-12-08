using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AdmBoots.Application.Users;
using AdmBoots.Application.Users.Dto;
using AdmBoots.Infrastructure.Authorization;
using AdmBoots.Infrastructure.Framework.Abstractions;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace AdmBoots.Api.Controllers {

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    [AllowAnonymous]
    public class LoginController : ControllerBase {
        private readonly IUserService _userService;
        private readonly IDistributedCache _cache;

        public LoginController(IUserService userService, IDistributedCache cache) {
            _userService = userService;
            _cache = cache;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInput input) {
            if (string.IsNullOrWhiteSpace(input.UserName)
                || string.IsNullOrWhiteSpace(input.Password)) {
                return Ok(ResponseBody.Bad("用户名或密码不能为空"));
            }
            var user = await _userService.LonginAsync(input);
            if (user != null) {
                return Ok(ResponseBody.From(GetAuthenticateInfo(user)));
            }
            return Ok(ResponseBody.Bad("用户名或密码错误"));
        }

        /// <summary>
        /// 请求刷新Token（以旧换新）
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("refresh-token/{token}")]
        public async Task<object> RefreshToken(string token) {
            if (string.IsNullOrEmpty(token)) {
                return Ok(ResponseBody.Bad("令牌无效，请重新登陆"));
            }
            var uid = JwtToken.ReadJwtToken<int>(token);

            var user = await _userService.GetLoginUserAsync(uid);
            if (user != null) {
                return Ok(ResponseBody.From(GetAuthenticateInfo(user)));
            }
            return Ok(ResponseBody.Bad("认证失败"));
        }

        private AuthenticateInfo GetAuthenticateInfo(LoginUserInfo user) {
            var subjectId = user.Id.ToString();
            var claims = new List<Claim> {
                    new Claim(ClaimTypes.NameIdentifier, subjectId),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Expiration, DateTime.UtcNow.AddSeconds(TokenAuthConfiguration.Expiration.TotalSeconds).ToString()),
                    new Claim(JwtRegisteredClaimNames.AuthTime, DateTime.UtcNow.ToString("o")),
                };

            claims.AddRange(user.Roles.Select(s => new Claim(ClaimTypes.Role, s.Id.ToString())));
            var token = JwtToken.IssueToken(claims);

            if (_cache != null) {
                //token白名单  一个用户只有一个token有效
                //需要启用AuthorizationSetup中代码
                //30天清理失效得key
                _cache.SetString(subjectId, token, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(30) });
            }
            return new AuthenticateInfo() {
                AccessToken = token,
                TokenType = "Bearer",
                ExpireInSeconds = TokenAuthConfiguration.Expiration.TotalSeconds,
                UserName = user.UserName,
                IsMaster = user.IsMaster,
                Name = user.Name,
                UserId = user.Id,
                Roles = user.Roles
            };
        }
    }

    public class AuthenticateInfo {

        /// <summary>
        /// 认可标志
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Token类型
        /// </summary>
        public string TokenType { get; set; } = "Bear";

        /// <summary>
        /// 过期时间（单位：秒）
        /// </summary>
        public double ExpireInSeconds { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否为系统管理员
        /// </summary>
        public bool IsMaster { get; set; }

        public IList<UserRoles> Roles { get; set; }
    }
}
