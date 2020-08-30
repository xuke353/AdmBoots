using AdmBoots.Application.Users.Dto;
using AdmBoots.Infrastructure.Framework.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AdmBoots.Application.Users {

    public interface IUserService : ITransientDependency {


        Task<LoginUserInfo> LonginAsync(LoginInput input);

        Task<LoginUserInfo> GetLoginUserAsync(int id);

        Page<GetUserOutput> GetUserList(GetUserInput input);

        Task AddOrUpdateUser(int? id, AddOrUpdateUserInput input);

        Task DeleteUser(int[] ids);

        Task ResetPassword(int id);

        Task ModifyPassword(ModifyPasswordInput input);
    }
}
