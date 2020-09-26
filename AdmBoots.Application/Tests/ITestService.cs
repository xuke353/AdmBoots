
using AdmBoots.Infrastructure.Framework.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdmBoots.Application.Tests.Dto;
namespace AdmBoots.Application.Tests
{
	/// <summary>
	///
	/// </summary>
    public interface ITestService :ITransientDependency
	{
        Task<Page<GetTestOutput>> GetTestList(GetTestInput input);

        Task AddOrUpdateTest(int? id, AddOrUpdateTestInput input);

        Task DeleteTest(int[] ids);
    }
}
                    