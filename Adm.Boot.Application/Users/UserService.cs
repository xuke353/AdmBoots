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

namespace Adm.Boot.Application.Users {

    internal class UserService : AppServiceBase, IUserService {
        private readonly IRepository<User, int> _userRepository;
        private readonly IMapper _mapper;

        public UserService(IRepository<User, int> userRepository, IMapper mapper) {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public void AddUser() {
            var u = new User {
                Status = 1,
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
                Status = 1,
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
                Status = 1,
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
    }
}
