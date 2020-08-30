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
                    new Claim(JwtRegisteredClaimNames.Sub, subjectId),
                    new Claim(ClaimTypes.NameIdentifier, subjectId),
                    new Claim(JwtRegisteredClaimNames.AuthTime, DateTime.Now.ToString()),//DateTime.UtcNow.ToString("o")
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(expirationSeconds).ToString())
                };
            if (!string.IsNullOrWhiteSpace(user.Name)) {
                claims.Add(new Claim(JwtClaimTypes.Name, user.Name));
            }
            claims.AddRange(user.Roles.Select(s => new Claim(ClaimTypes.Role, s.Id.ToString())));

            var token = JwtToken.IssueToken(claims.ToArray(), requirement);

            if (cache != null) {
                //token白名单  一个用户只有一个token有效
                //30天清理失效得key
                cache.SetString(subjectId, token, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(30) });
            }
            return new AuthenticateInfo() {
                AccessToken = token,
                TokenType = "Bearer",
                ExpireInSeconds = expirationSeconds,
                UserInfo = user
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

        public LoginUserInfo UserInfo { get; set; }
    }
}
