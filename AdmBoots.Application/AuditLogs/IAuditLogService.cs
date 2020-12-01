using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdmBoots.Application.Auditings.Dto;
using AdmBoots.Application.AuditLogs.Dto;
using AdmBoots.Infrastructure.Framework.Abstractions;

namespace AdmBoots.Application.Auditings {

    public interface IAuditLogService : ITransientDependency {

        Task SaveAsync(AuditInfo auditInfo);

        Page<GetAuditLogOutput> GetAuditLogPage(PageRequest input);
    }
}
