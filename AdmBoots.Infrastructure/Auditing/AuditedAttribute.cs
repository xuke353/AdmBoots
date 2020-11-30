using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmBoots.Infrastructure.Auditing {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class AuditedAttribute : Attribute {

    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class DisableAuditingAttribute : Attribute {

    }
}
