using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AdmBoots.Quartz.Dto;

namespace AdmBoots.Quartz {
    public interface ISchedulerCenter {
        /// <summary>
        /// 添加工作调度（insert to DB）
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public Task AddJobAsync(AddScheduleInput scheduleInput);
        /// <summary>
        /// 删除 指定的计划
        /// </summary>
        /// <param name="jobGroup"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public Task DeleteJobAsync(string jobName, string groupName);
        /// <summary>
        /// 修改计划
        /// </summary>
        /// <param name="scheduleInput"></param>
        /// <returns></returns>
        public Task UpdateJobAsync(AddScheduleInput scheduleInput);
        /// <summary>
        /// 暂停 指定的计划
        /// </summary>
        /// <param name="jobGroup"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public Task PauseJobAsync(string jobName, string groupName);
        /// <summary>
        /// 恢复运行暂停的任务
        /// </summary>
        /// <param name="jobGroup"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public Task ResumeJobAsync(string jobName, string groupName);
        /// <summary>
        /// 立即执行
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public Task RunJobAsync(string jobName, string groupName);
        /// <summary>
        /// 获取Job日志（JobDetail中的）
        /// </summary>
        /// <param name="jobGroup"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>

        public Task<List<string>> GetJobLogsAsync(string jobName, string groupName);
        /// <summary>
        /// 获取Job列表
        /// </summary>
        /// <returns></returns>
        public Task<List<GetScheduleOutput>> GetJobListAsync();

        /// <summary>
        /// 开启任务调度器
        /// </summary>
        /// <returns></returns>
        public Task<bool> Start();
    }
}
