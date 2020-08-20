using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Adm.Boot.Data.EntityFrameworkCore.Uow {

    public static class UnitOfWorkExtensions {

        public static UnitOfWorkAttribute GetUnitOfWorkAttributeOrNull(this IAdmUnitOfWork unitOfWork, MethodInfo methodInfo) {
            var attrs = methodInfo.GetCustomAttributes(true).OfType<UnitOfWorkAttribute>().ToArray();
            if (attrs.Length > 0) {
                return attrs[0];
            }

            attrs = methodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(true).OfType<UnitOfWorkAttribute>().ToArray();
            if (attrs.Length > 0) {
                return attrs[0];
            }
            return null;
        }
    }
}
