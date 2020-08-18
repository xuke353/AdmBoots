using Adm.Boot.Application.Users.Dto;
using Adm.Boot.Domain.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Adm.Boot.Application
{
    public class AutoMapProfile : Profile
    {
        /// <summary>
        /// 配置构造函数，用来创建关系映射
        /// </summary>
        public AutoMapProfile()
        {
            CreateMap<User, GetUserOutput>(MemberList.Source);
        }
    }
}
