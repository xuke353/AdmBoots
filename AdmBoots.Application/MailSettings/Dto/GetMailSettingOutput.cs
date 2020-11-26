using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Domain.Models;
using AutoMapper;

namespace AdmBoots.Application.MailSettings.Dto {
    [AutoMap(typeof(MailSetting))]
    public class GetMailSettingOutput : MailSetting {
    }
}
