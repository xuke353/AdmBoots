using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdmBoots.Application.Auditings.Dto;

namespace AdmBoots.Application.Auditings {

    public interface IAuditLogService : ITransientDependency {
        Task SaveAsync(AuditInfo auditInfo);
    }
}
