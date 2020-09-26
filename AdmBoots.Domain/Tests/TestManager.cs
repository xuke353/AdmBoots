using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AdmBoots.Domain.IRepositories;
using AdmBoots.Domain.Models;

namespace AdmBoots.Domain.Tests {
    public class TestManager : ITestManager {
        private readonly IRepository<Test, int> _testRepository;
        public TestManager(IRepository<Test, int> testRepository) {
            _testRepository = testRepository;
        }

        public async Task InsertTest(Test test) {
            await _testRepository.InsertAsync(test);
        }

        public async Task UpdateTest(Test test) {
            test.Age = 18;
            await _testRepository.UpdateAsync(test);
        }
    }
}
