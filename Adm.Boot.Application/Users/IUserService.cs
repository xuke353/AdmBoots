using Adm.Boot.Application.Users.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Adm.Boot.Application.Users {

    public interface IUserService : ITransientDependency {

        IList<GetUserOutput> GetUserList();

        void AddUser();

        Task AddUserAsync();
    }
}
