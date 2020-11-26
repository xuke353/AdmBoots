using AdmBoots.Application.MailSettings.Dto;
using AdmBoots.Application.Menus.Dto;
using AdmBoots.Application.Roles.Dto;
using AdmBoots.Application.Tests.Dto;
using AdmBoots.Application.Users.Dto;
using AdmBoots.Domain.Models;
using AutoMapper;

namespace AdmBoots.Application {

    public class AutoMapProfile : Profile {

        /// <summary>
        /// 配置构造函数，用来创建关系映射
        /// </summary>
        public AutoMapProfile() {
            CreateMap<User, LoginUserInfo>(MemberList.Source);
            CreateMap<User, GetUserOutput>(MemberList.Source);
            CreateMap<Role, GetRoleOutput>(MemberList.Source);
            CreateMap<Menu, GetTreeMenuOutput>();

            CreateMap<AddOrUpdateMenuInput, Menu>();

            //以下使用AutoMapAttribute映射，详见Dto
            //CreateMap<AddMailSettingInput, MailSetting>();
            //CreateMap<MailSetting, GetMailSettingOutput>();

            //
            CreateMap<Test, GetTestOutput>();
        }
    }
}
