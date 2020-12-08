using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AdmBoots.Infrastructure.Authorization {

    public class JwtToken {

        /// <summary>
        /// 获取基于JWT的Token
        /// </summary>
        /// <param name="claims">需要在登陆的时候配置</param>
        /// <param name="requirement">在startup中定义的参数</param>
        /// <returns></returns>
        public static string IssueToken(List<Claim> claims) {
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);
            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            });

            var now = DateTime.UtcNow;
            // 实例化JwtSecurityToken
            var jwt = new JwtSecurityToken(
                issuer: TokenAuthConfiguration.Issuer,
                audience: TokenAuthConfiguration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(TokenAuthConfiguration.Expiration),
                signingCredentials: TokenAuthConfiguration.SigningCredentials
            );
            // 生成 Token
            return new JwtSecurityTokenHandler().WriteToken(jwt); ;
        }

        /// <summary>
        /// 解析Token
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public static T ReadJwtToken<T>(string jwtStr) {
            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);
            var uid = jwtToken.Claims.FirstOrDefault(m => m.Type == JwtRegisteredClaimNames.Sub).Value;
            return (T)Convert.ChangeType(uid, typeof(T));
        }
    }
}
