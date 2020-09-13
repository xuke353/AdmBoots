using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdmBoots.Data.EntityFrameworkCore;
using AdmBoots.Domain.Models;
using AdmBoots.Infrastructure;
using AdmBoots.Infrastructure.CustomExceptions;
using AdmBoots.Infrastructure.Extensions;
using AdmBoots.Infrastructure.Framework.Abstractions;
using AdmBoots.Quartz.Common;
using AdmBoots.Quartz.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using Quartz.Spi;

namespace AdmBoots.Quartz {

    public class SchedulerCenter : ISchedulerCenter {
        private readonly IScheduler _scheduler;

        public SchedulerCenter(ISchedulerFactory schedulerFactory, IJobFactory jobFactory) {
            _scheduler = schedulerFactory.GetScheduler().Result;
            _scheduler.JobFactory = jobFactory;
        }

        public async Task AddJobAsync(AddScheduleInput scheduleInput) {
            if (await _scheduler.CheckExists(new JobKey(scheduleInput.JobName, scheduleInput.GroupName)))
                throw new BusinessException($"任务已存在");
            var validExpression = IsValidExpression(scheduleInput.Cron);
            if (!validExpression)
                throw new BusinessException($"请确认表达式{scheduleInput.Cron}是否正确!");
            scheduleInput.Status = (int)TriggerState.Normal;
            //http请求配置
            var httpDir = new Dictionary<string, string>()
            {
                    { QuartzConstant.REQUESTURL,scheduleInput.RequestUrl},
                    { QuartzConstant.REQUESTPARAMS,scheduleInput.RequestParams},
                    { QuartzConstant.REQUESTTYPE, ((int)scheduleInput.RequestType).ToString()},
                    { QuartzConstant.HEADERS, scheduleInput.Headers},
                    { QuartzConstant.CREATETIME, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},
            };
            // 定义这个工作，并将其绑定到我们的IJob实现类
            IJobDetail job = JobBuilder.Create<HttpJob>()
                .SetJobData(new JobDataMap(httpDir))
                .WithDescription(scheduleInput.Describe)
                .WithIdentity(scheduleInput.JobName, scheduleInput.GroupName)
                .Build();
            ITrigger trigger = TriggerBuilder.Create()
                   .WithIdentity(scheduleInput.JobName, scheduleInput.GroupName)
                   .StartNow()
                   .WithDescription(scheduleInput.Describe)
                   .WithCronSchedule(scheduleInput.Cron, cronScheduleBuilder => cronScheduleBuilder.WithMisfireHandlingInstructionFireAndProceed())//指定cron表达式
                   .ForJob(scheduleInput.JobName, scheduleInput.GroupName)//作业名称
                   .Build();

            await _scheduler.ScheduleJob(job, trigger);
            if (scheduleInput.Status == (int)TriggerState.Normal) {
                await _scheduler.Start();
            } else {
                await TriggerAction(scheduleInput.JobName, scheduleInput.GroupName, JobAction.暂停, scheduleInput);
                //FileQuartz.WriteStartLog($"作业:{taskOptions.TaskName},分组:{taskOptions.GroupName},新建时未启动原因,状态为:{taskOptions.Status}");
            }
        }

        public async Task DeleteJobAsync(string jobName, string groupName) {
            await TriggerAction(jobName, groupName, JobAction.删除);
        }

        public async Task UpdateJobAsync(AddScheduleInput scheduleInput) {
            await TriggerAction(scheduleInput.JobName, scheduleInput.GroupName, JobAction.修改, scheduleInput);
        }

        public async Task PauseJobAsync(string jobName, string groupName) {
            await TriggerAction(jobName, groupName, JobAction.暂停);
        }

        public async Task ResumeJobAsync(string jobName, string groupName) {
            await TriggerAction(jobName, groupName, JobAction.开启);
        }

        public async Task RunJobAsync(string jobName, string groupName) {
            await TriggerAction(jobName, groupName, JobAction.立即执行);
        }

        public async Task<List<GetScheduleOutput>> GetJobListAsync() {
            var list = new List<GetScheduleOutput>();
            var groups = await _scheduler.GetJobGroupNames();
            foreach (var groupName in groups) {
                foreach (var jobKey in await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName))) {
                    var triggers = await _scheduler.GetTriggersOfJob(jobKey);
                    var trigger = triggers.AsEnumerable().FirstOrDefault();
                    var jobDetail = await _scheduler.GetJobDetail(jobKey);
                    var createTimeStr = jobDetail.JobDataMap.GetString(QuartzConstant.CREATETIME);
                    list.Add(new GetScheduleOutput {
                        JobName = jobKey.Name,
                        GroupName = jobKey.Group,
                        Cron = (trigger as CronTriggerImpl)?.CronExpressionString,
                        Status = (int)(await _scheduler.GetTriggerState(trigger.Key)),
                        PreviousFireTime = trigger.GetPreviousFireTimeUtc()?.LocalDateTime,
                        NextFireTime = trigger.GetNextFireTimeUtc()?.LocalDateTime,
                        Describe = jobDetail.Description,
                        RequestUrl = jobDetail.JobDataMap.GetString(QuartzConstant.REQUESTURL),
                        RequestType = (RequestType)int.Parse(jobDetail.JobDataMap.GetString(QuartzConstant.REQUESTTYPE)),
                        Headers = jobDetail.JobDataMap.GetString(QuartzConstant.HEADERS),
                        RequestParams = jobDetail.JobDataMap.GetString(QuartzConstant.REQUESTPARAMS),
                        CreateTime = string.IsNullOrEmpty(createTimeStr) ? DateTime.Now : DateTime.Parse(jobDetail.JobDataMap.GetString(QuartzConstant.CREATETIME))
                    });
                }
            }
            return list.OrderByDescending(t => t.CreateTime).ToList();
        }

        public async Task<Page<JobLog>> GetJobLogsAsync(GetLogInput input) {
            if (string.IsNullOrEmpty(input.JobKey))
                throw new BusinessException($"JobKey(任务名.组名 不能为空)");
            var dbContext = AdmBootsApp.ServiceProvider.GetService(typeof(AdmDbContext)) as AdmDbContext;
            var result = dbContext.JobLogs.AsQueryable().Where(t => t.JobName == input.JobKey);
            var pageResult = result.PageAndOrderBy(input);
            return new Page<JobLog>(input, result.Count(), await pageResult.ToListAsync());
        }

        public async Task<bool> Start() {
            //开启调度器
            if (_scheduler.InStandbyMode) {
                var groups = await _scheduler.GetJobGroupNames();
                foreach (var groupName in groups) {
                    foreach (var jobKey in await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName))) {
                        var triggers = await _scheduler.GetTriggersOfJob(jobKey);
                        var trigger = triggers.AsEnumerable().FirstOrDefault();
                        var state = await _scheduler.GetTriggerState(trigger.Key);
                        if (state == TriggerState.Paused)
                            await _scheduler.ResumeTrigger(trigger.Key);
                    }
                }
                await _scheduler.Start();
                ">>>>调度器启动".WriteSuccessLine();
            }
            return _scheduler.InStandbyMode;
        }

        /// <summary>
        /// 触发新增、删除、修改、暂停、启用、立即执行事件
        /// </summary>
        /// <param name="schedulerFactory"></param>
        /// <param name="taskName"></param>
        /// <param name="groupName"></param>
        /// <param name="action"></param>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public async Task TriggerAction(string taskName, string groupName, JobAction action, AddScheduleInput scheduleInput = null) {
            if (string.IsNullOrEmpty(taskName) || string.IsNullOrEmpty(groupName))
                throw new BusinessException($"任务名或组名不能为空");
            var scheduler = _scheduler;
            var jobKey = new JobKey(taskName, groupName);
            var triggerKey = new TriggerKey(taskName, groupName);
            if (!await scheduler.CheckExists(jobKey))
                throw new BusinessException($"未发现任务");
            if (!await scheduler.CheckExists(triggerKey))
                throw new BusinessException($"未发现触发器");

            switch (action) {
                case JobAction.删除:
                case JobAction.修改:
                    await scheduler.PauseTrigger(triggerKey);
                    await scheduler.UnscheduleJob(triggerKey);// 移除触发器
                    await scheduler.DeleteJob(jobKey);
                    if (action == JobAction.修改) {
                        await AddJobAsync(scheduleInput);
                        $">>>>{triggerKey.ToString()}修改成功".WriteSuccessLine();
                    }
                    break;

                case JobAction.暂停:
                    await scheduler.PauseTrigger(triggerKey);
                    $">>>>{triggerKey.ToString()}暂停".WriteSuccessLine();
                    break;

                case JobAction.停止:
                    await scheduler.Shutdown();
                    $">>>>{triggerKey.ToString()}停止".WriteSuccessLine();
                    break;

                case JobAction.开启:
                    await scheduler.ResumeTrigger(triggerKey);
                    $">>>>{triggerKey.ToString()}开启".WriteSuccessLine();
                    break;

                case JobAction.立即执行:
                    await scheduler.TriggerJob(jobKey);
                    $">>>>{triggerKey.ToString()}立即执行".WriteSuccessLine();
                    break;
            }
        }

        private bool IsValidExpression(string cronExpression) {
            try {
                CronTriggerImpl trigger = new CronTriggerImpl();
                trigger.CronExpressionString = cronExpression;
                DateTimeOffset? date = trigger.ComputeFirstFireTimeUtc(null);
                return date != null;
            } catch (Exception e) {
                throw new BusinessException($"请确认表达式{cronExpression}是否正确!{e.Message}");
            }
        }
    }
}
