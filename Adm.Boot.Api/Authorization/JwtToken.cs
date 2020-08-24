using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Adm.Boot.Infrastructure.Authorization;

namespace Adm.Boot.Api.Authorization {
    public class JwtToken {
        /// <summary>
        /// 获取基于JWT的Token
        /// </summary>
        /// <param name="claims">需要在登陆的时候配置</param>
        /// <param name="requirement">在startup中定义的参数</param>
        /// <returns></returns>
        public static string IssueToken(Claim[] claims, AdmPolicyRequirement requirement) {
            var now = DateTime.Now;
            // 实例化JwtSecurityToken
            var jwt = new JwtSecurityToken(
                issuer: requirement.Issuer,
                audience: requirement.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(requirement.Expiration),
                signingCredentials: requirement.SigningCredentials
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
