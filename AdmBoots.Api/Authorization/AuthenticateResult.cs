using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AdmBoots.Application.Users.Dto;
using AdmBoots.Infrastructure.Authorization;
using IdentityModel;
using Microsoft.Extensions.Caching.Distributed;

namespace AdmBoots.Api.Authorization {

    public class AuthenticateResult {

        public static AuthenticateInfo Get(LoginUserInfo user, AdmPolicyRequirement requirement, IDistributedCache cache = null) {
            var expirationSeconds = requirement.Expiration.TotalSeconds;
            var subjectId = user.Id.ToString();
            var claims = new List<Claim> {
                    new Claim(ClaimTypes.NameIdentifier, subjectId),//必须提供NameIdentifier值，否则AdmSeesion取不到UserId
                    new Claim(JwtRegisteredClaimNames.AuthTime, DateTime.UtcNow.ToString("o")),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtClaimTypes.Name, user.Name),
                    new Claim(JwtClaimTypes.Expiration, DateTime.Now.AddSeconds(expirationSeconds).ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, subjectId),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
                };

            claims.AddRange(user.Roles.Select(s => new Claim(JwtClaimTypes.Role, s.Id.ToString())));

            var token = JwtToken.IssueToken(claims.ToArray(), requirement);

            if (cache != null) {
                //token白名单  一个用户只有一个token有效
                //需要启用AuthorizationSetup中代码
                //30天清理失效得key
                cache.SetString(subjectId, token, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(30) });
            }
            return new AuthenticateInfo() {
                AccessToken = token,
                TokenType = "Bearer",
                ExpireInSeconds = expirationSeconds,
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
