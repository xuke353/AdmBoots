using System;
using System.Collections.Generic;
using System.Text;

namespace AdmBoots.Infrastructure.Framework.Web {
    public interface IAdmSession<TKey> {
        /// <summary>
        /// 用户Id
        /// </summary>
        TKey UserId { get; }


        /// <summary>
        /// 由字母或数字组成的用户名称，以标明用户的身份
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// 名字
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 是否认证
        /// </summary>
        bool IsAuthenticated { get; }
    }

    public interface IAdmSession : IAdmSession<int> {
    }
}
