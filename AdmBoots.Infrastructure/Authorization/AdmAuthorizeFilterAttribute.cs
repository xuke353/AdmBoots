using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmBoots.Infrastructure.Authorization {

    [AttributeUsage(AttributeTargets.Method)]
    public class AdmAuthorizeFilterAttribute : Attribute {

        /// <summary>
        /// 资源标识名称
        /// </summary>
        public string FilterName { get; set; }

        public AdmAuthorizeFilterAttribute(string filterName) {
            FilterName = filterName;
        }
    }
}
