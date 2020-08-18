using Adm.Boot.Application.Users.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using Adm.Boot.Domain.Models;
using Adm.Boot.Domain.IRepositories;
using AutoMapper;
using System.Linq;

namespace Adm.Boot.Application.Users {
    class UserService : AppServiceBase, IUserService {
        private readonly IRepository<User, int> _userRepository;
        private readonly IMapper _mapper;

        public UserService(IRepository<User, int> userRepository, IMapper mapper) {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public IList<GetUserOutput> GetUserList() {
            var queryResult = _userRepository.GetAll().ToList();
            var outputUser = _mapper.Map<List<GetUserOutput>>(queryResult);
            return outputUser;
        }
    }
}
