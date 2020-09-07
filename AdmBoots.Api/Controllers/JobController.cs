using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdmBoots.Infrastructure.Framework.Abstractions;
using AdmBoots.Quartz;
using AdmBoots.Quartz.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdmBoots.Api.Controllers {

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/jobs")]
    [Authorize]
    public class JobController : ControllerBase {
        private readonly ISchedulerCenter _schedulerCenter;

        public JobController(ISchedulerCenter schedulerCenter) {
            _schedulerCenter = schedulerCenter;
        }

        /// <summary>
        /// 获取所有的作业
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetJobs() {
            var result = await _schedulerCenter.GetJobListAsync();
            return Ok(ResponseBody.From(result));
        }

        /// <summary>
        /// 获取作业运行日志
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("logs")]
        public async Task<IActionResult> GetJobLog([FromQuery]GetLogInput input) {
            var result = await _schedulerCenter.GetJobLogsAsync(input);
            return Ok(ResponseBody.From(result));
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]AddScheduleInput input) {
            await _schedulerCenter.AddJobAsync(input);
            return Ok(ResponseBody.From("添加成功"));
        }

        [HttpDelete]
        public async Task<IActionResult> Remove([FromBody]JobKeyInput input) {
            await _schedulerCenter.DeleteJobAsync(input.JobName, input.GroupName);
            return Ok(ResponseBody.From("删除成功"));
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody]AddScheduleInput input) {
            await _schedulerCenter.UpdateJobAsync(input);
            return Ok(ResponseBody.From("修改成功"));
        }

        [HttpPut("pause")]
        public async Task<IActionResult> Pause([FromBody]JobKeyInput input) {
            await _schedulerCenter.PauseJobAsync(input.JobName, input.GroupName);
            return Ok(ResponseBody.From("暂停成功"));
        }

        /// <summary>
        /// 恢复启动
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("pesume")]
        public async Task<IActionResult> Resume([FromBody]JobKeyInput input) {
            await _schedulerCenter.ResumeJobAsync(input.JobName, input.GroupName);
            return Ok(ResponseBody.From("启动成功"));
        }

        [HttpPut("run")]
        public async Task<IActionResult> Run([FromBody]JobKeyInput input) {
            await _schedulerCenter.RunJobAsync(input.JobName, input.GroupName);
            return Ok(ResponseBody.From("执行成功"));
        }
    }
}
