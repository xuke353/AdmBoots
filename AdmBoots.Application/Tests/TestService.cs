
using AdmBoots.Application.Tests.Dto;
using AdmBoots.Domain.IRepositories;
using AdmBoots.Domain.Models;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdmBoots.Infrastructure.Framework.Abstractions;
using AdmBoots.Domain.Tests;

namespace AdmBoots.Application.Tests {
    /// <summary>
    ///                                                                                                                                                                                                                                  
    /// </summary>
    public class TestService : AppServiceBase, ITestService {
        //调用Domain层做有状态的逻辑处理
        //private readonly ITestManager _testManager;

        //public TestService(ITestManager testManager) {
        //    _testManager = testManager;
        //}

        private readonly IRepository<Test, int> _testRepository;
        public TestService(IRepository<Test, int> testRepository) {
            _testRepository = testRepository;
        }
        public Task AddOrUpdateTest(int? id, AddOrUpdateTestInput input) {
            var entity = ObjectMapper.Map<Test>(input);
            throw new System.NotImplementedException();
        }

        public Task DeleteTest(int[] ids) {
            throw new System.NotImplementedException();
        }

        public Task<Page<GetTestOutput>> GetTestList(GetTestInput input) {
            throw new System.NotImplementedException();
        }
    }
}
