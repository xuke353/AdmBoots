using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Application.MailSettings.Dto;

namespace AdmBoots.Application.MailSettings {
    public interface IMailSettingService : ITransientDependency {
        void AddMailSetting(AddMailSettingInput input);

        GetMailSettingOutput GetMailSetting();
    }
}
