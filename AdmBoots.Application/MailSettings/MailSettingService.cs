using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdmBoots.Application.MailSettings.Dto;
using AdmBoots.Data.EntityFrameworkCore.Uow;
using AdmBoots.Domain.IRepositories;
using AdmBoots.Domain.Models;

namespace AdmBoots.Application.MailSettings {
    public class MailSettingService : AppServiceBase, IMailSettingService {
        private readonly IRepository<MailSetting, int> _mailSettingRepository;

        public MailSettingService(IRepository<MailSetting, int> mailSettingRepository) {
            _mailSettingRepository = mailSettingRepository;
        }

        public void AddMailSetting(AddMailSettingInput input) {
            var result = _mailSettingRepository.GetAll().FirstOrDefault();
            if (result != null) {
                var updateEntity = ObjectMapper.Map(input, result);
                _mailSettingRepository.Update(updateEntity);
            } else {
                var mailSetting = ObjectMapper.Map<MailSetting>(input);
                _mailSettingRepository.Insert(mailSetting);
            }
        }
        [UnitOfWork(IsDisabled = true)]
        public GetMailSettingOutput GetMailSetting() {
            var mailSetting = _mailSettingRepository.GetAll().FirstOrDefault();
            return mailSetting != null
                ? ObjectMapper.Map<GetMailSettingOutput>(mailSetting)
                : new GetMailSettingOutput();
        }
    }
}
