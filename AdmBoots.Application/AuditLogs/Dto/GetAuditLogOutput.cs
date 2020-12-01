using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdmBoots.Domain.Models;
using AutoMapper;

namespace AdmBoots.Application.AuditLogs.Dto {

    [AutoMap(typeof(AuditLog))]
    public class GetAuditLogOutput : AuditLog {
    }
}
