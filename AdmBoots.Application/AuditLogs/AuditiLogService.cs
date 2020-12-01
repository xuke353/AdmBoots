using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdmBoots.Application.Auditings.Dto;
using AdmBoots.Application.AuditLogs.Dto;
using AdmBoots.Domain;
using AdmBoots.Domain.IRepositories;
using AdmBoots.Domain.Models;
using AdmBoots.Infrastructure.Extensions;
using AdmBoots.Infrastructure.Framework.Abstractions;
using AutoMapper;

namespace AdmBoots.Application.Auditings {

    public class AuditiLogService : AppServiceBase, IAuditLogService {
        private readonly IRepository<AuditLog, int> _auditLogRepository;

        public AuditiLogService(IRepository<AuditLog, int> auditLogRepository) {
            _auditLogRepository = auditLogRepository;
        }

        public Page<GetAuditLogOutput> GetAuditLogPage(PageRequest input) {
            var result = _auditLogRepository.GetAll();
            var pageResult = result.PageAndOrderBy(input);
            var output = ObjectMapper.Map<List<GetAuditLogOutput>>(pageResult.ToList());
            return new Page<GetAuditLogOutput>(input, result.Count(), output);
        }

        public Task SaveAsync(AuditInfo auditInfo) {
            var auditLog = ObjectMapper.Map<AuditLog>(auditInfo);
            return _auditLogRepository.InsertAsync(auditLog);
        }
    }
}
