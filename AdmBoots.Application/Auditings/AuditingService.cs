using System.Threading.Tasks;
using AdmBoots.Application.Auditings.Dto;
using AdmBoots.Domain;
using AdmBoots.Domain.IRepositories;
using AdmBoots.Domain.Models;
using AutoMapper;

namespace AdmBoots.Application.Auditings {

    public class AuditingService : AppServiceBase, IAuditingService {
        private readonly IRepository<AuditLog, int> _auditLogRepository;

        public AuditingService(IRepository<AuditLog, int> auditLogRepository) {
            _auditLogRepository = auditLogRepository;
        }

        public Task SaveAsync(AuditInfo auditInfo) {
            var auditLog = ObjectMapper.Map<AuditLog>(auditInfo);
            return _auditLogRepository.InsertAsync(auditLog);
        }
    }
}
