using System;
using System.Text;
using AdmBoots.Infrastructure.Config;
using AdmBoots.Infrastructure.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace AdmBoots.Infrastructure.Authorization {

    public static class TokenAuthConfiguration {

        /// <summary>
        /// 发行人
        /// </summary>
        public static string Issuer { get; }

        /// <summary>
        /// 订阅人
        /// </summary>
        public static string Audience { get; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public static TimeSpan Expiration { get; }

        /// <summary>
        /// 安全密钥
        /// </summary>
        public static SecurityKey IssuerSigningKey { get; }

        /// <summary>
        /// 签名验证
        /// </summary>
        public static SigningCredentials SigningCredentials { get; }

        static TokenAuthConfiguration() {
            var section = CfgManager.Configuration.GetSection("Authentication:JwtBearer");
            Issuer = section["Issuer"];
            Audience = section["Audience"];
            Expiration = TimeSpan.FromMinutes(1);//TimeSpan.FromHours(section["Expiration"].ObjToMoney(24));
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(section["SecurityKey"]));
            SigningCredentials = new SigningCredentials(IssuerSigningKey, SecurityAlgorithms.HmacSha256);
        }
    }
}
