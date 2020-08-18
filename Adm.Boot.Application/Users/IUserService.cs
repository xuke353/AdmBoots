using Adm.Boot.Application.Users.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Adm.Boot.Application.Users {

    public interface IUserService : ITransientDependency {
        IList<GetUserOutput> GetUserList();
    }
}
