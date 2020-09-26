using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AdmBoots.Domain.Models;

namespace AdmBoots.Domain.Tests {
    public interface ITestManager {
        Task InsertTest(Test test);

        Task UpdateTest(Test test);
    }
}
