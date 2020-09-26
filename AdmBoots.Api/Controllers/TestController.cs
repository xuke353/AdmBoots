
using AdmBoots.Application.Tests;
using AdmBoots.Application.Tests.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AdmBoots.Infrastructure.Domain;
using AdmBoots.Infrastructure.Framework.Abstractions;

namespace AdmBoots.Api.Controllers
{
	/// <summary>
	///
	/// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/tests")]
    [Authorize(AdmConsts.POLICY)]
    public class TestController : ControllerBase
	{
        private readonly ITestService _testService;
        public TestController(ITestService testService)
        {
            _testService = testService;
        }

        [HttpGet]
        public IActionResult GetTestList([FromQuery]GetTestInput input){
                    var result = _testService.GetTestList(input);
                    return Ok(ResponseBody.From(result));
                }
        [HttpPost]
        public async Task<IActionResult> AddTest([FromBody]AddOrUpdateTestInput input){
            await _testService.AddOrUpdateTest(null, input);
            return Ok(ResponseBody.From("保存成功"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTest(int id, [FromBody]AddOrUpdateTestInput input) {
            await _testService.AddOrUpdateTest(id, input);
            return Ok(ResponseBody.From("修改成功"));
         }

        [HttpDelete]
        public async Task<IActionResult> DeleteTest(int[] ids) {
            await _testService.DeleteTest(ids);
            return Ok(ResponseBody.From("删除成功"));
        }
    }
}