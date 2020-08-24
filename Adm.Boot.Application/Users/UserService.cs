using Adm.Boot.Application.Users.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using Adm.Boot.Domain.Models;
using Adm.Boot.Domain.IRepositories;
using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using Adm.Boot.Infrastructure.CustomExceptions;
using Adm.Boot.Data.EntityFrameworkCore.Uow;
using Adm.Boot.Domain;
using Adm.Boot.Infrastructure.Helper;

namespace Adm.Boot.Application.Users {

    internal class UserService : AppServiceBase, IUserService {
        private readonly IRepository<User, int> _userRepository;
        private readonly IRepository<Role, int> _roleRepository;
        private readonly IRepository<UserRole, int> _userRoleRepository;

        public UserService(IRepository<User, int> userRepository, IRepository<Role, int> roleRepository, IRepository<UserRole, int> userRoleRepository) {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
        }

        public void AddUser() {
            var u = new User {
                Status = SysStatus.有效,
                CreateTime = DateTime.Now,
                Email = "xkry353@163.com",
                IsMaster = true,
                Name = "艾登",
                Password = "123456",
                UserName = "Aiden"
            };
            _userRepository.Insert(u);
        }

        public async Task AddUserAsync() {
            var u = new User {
                Status = SysStatus.有效,
                CreateTime = DateTime.Now,
                Email = "xkry353@163.com",
                IsMaster = true,
                Name = "艾登",
                Password = "123456",
                UserName = "Aiden"
            };
            var b = await _userRepository.InsertAsync(u);
            CurrentUnitOfWork.SaveChanges();
            //saveChanges后才能获取b
            var u2 = new User {
                Status = SysStatus.有效,
                CreateTime = DateTime.Now,
                Email = "xkry353@163.com",
                IsMaster = true,
                Name = "艾伦",
                Password = "123456",
                UserName = "Ailen"
            };
            await _userRepository.InsertAsync(u2);
        }



        [UnitOfWork(IsDisabled = true)]
        public IList<GetUserOutput> GetUserList() {
            var queryResult = _userRepository.GetAll().ToList();
            var outputUser = ObjectMapper.Map<List<GetUserOutput>>(queryResult);
            return outputUser;
        }

        public async Task<LoginUserInfo> LonginAsync(LoginInput input) {
            var md5Pwd = MD5Helper.MD5Encrypt32(input.Password);
            var users = await _userRepository.GetAllListAsync(u => u.UserName == input.UserName && u.Password == md5Pwd && u.Status == SysStatus.有效);
            if (users.Count > 0) {
                var user = users.FirstOrDefault();
                user.LastLoginTime = DateTime.Now;
                _userRepository.Update(user);

                var userInfo = ObjectMapper.Map<LoginUserInfo>(user);
                userInfo.Roles = userInfo.Roles = GetUserRoles(user.Id); ;
                return userInfo;
            }
            return null;
        }
        [UnitOfWork(IsDisabled = true)]
        public async Task<LoginUserInfo> GetLoginUserAsync(int id) {
            var user = await _userRepository.FirstOrDefaultAsync(t => t.Id == id);
            if (user != null) {
                var userInfo = ObjectMapper.Map<LoginUserInfo>(user);
                userInfo.Roles = GetUserRoles(user.Id);
                return userInfo;
            }
            return null;
        }
        /// <summary>
        /// 获取用户的角色集合
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private List<UserRoles> GetUserRoles(int userId) {
            var userRoles = (from u in _userRepository.GetAll()
                             join ur in _userRoleRepository.GetAll()
                             on u.Id equals ur.UserId
                             join r in _roleRepository.GetAll()
                             on ur.RoleId equals r.Id
                             where u.Id == userId
                             select new UserRoles {
                                 Id = r.Id,
                                 Code = r.Code,
                                 Name = r.Name
                             }).ToList();
            return userRoles;
        }
    }
}
