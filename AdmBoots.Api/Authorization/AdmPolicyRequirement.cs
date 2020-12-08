using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace AdmBoots.Infrastructure.Authorization {

    /// <summary>
    /// 角色与接口的权限要求参数
    /// 继承 IAuthorizationRequirement，用于设计自定义权限处理器AdmAuthorizationHandler
    /// </summary>
    public class AdmPolicyRequirement : IAuthorizationRequirement {

        /// <summary>
        /// 认证授权类型
        /// </summary>
        public string ClaimType { internal get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="claimType">声明类型</param>
        public AdmPolicyRequirement(string claimType) {
            ClaimType = claimType;
        }
    }
}
