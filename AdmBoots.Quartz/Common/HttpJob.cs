using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AdmBoots.Data.EntityFrameworkCore;
using AdmBoots.Data.EntityFrameworkCore.Uow;
using AdmBoots.Domain.Models;
using AdmBoots.Infrastructure;
using AdmBoots.Infrastructure.Extensions;
using AdmBoots.Infrastructure.Framework.Abstractions;
using AdmBoots.Infrastructure.Mail;
using AdmBoots.Quartz.Dto;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Quartz;

namespace AdmBoots.Quartz.Common {

    //不允许并发执行
    [DisallowConcurrentExecution]
    //在成功执行 execute() 方法后(没有抛出异常)更新 JobDetail 的 JobDataMap
    [PersistJobDataAfterExecution]
    public class HttpJob : IJob {
        private readonly IServiceProvider _serviceProvider;

        public HttpJob(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context) {
            var warnTime = 20;//接口请求超过多少秒记录警告日志
            //获取相关参数
            var requestUrl = context.JobDetail.JobDataMap.GetString(QuartzConstant.REQUESTURL);
            requestUrl = requestUrl?.IndexOf("http") == 0 ? requestUrl : "http://" + requestUrl;
            var requestParameters = context.JobDetail.JobDataMap.GetString(QuartzConstant.REQUESTPARAMS);
            var headersString = context.JobDetail.JobDataMap.GetString(QuartzConstant.HEADERS);
            var headers = headersString != null ? JsonConvert.DeserializeObject<Dictionary<string, string>>(headersString?.Trim()) : null;
            var requestType = (RequestType)int.Parse(context.JobDetail.JobDataMap.GetString(QuartzConstant.REQUESTTYPE));

            var stopwatch = new Stopwatch();
            stopwatch.Restart(); //  开始监视代码运行时间
            var response = new HttpResponseMessage();

            var loginfo = new JobLog {
                BeginTime = DateTime.Now,
                JobName = context.JobDetail.Key.ToString(),
                Level = "消息"
            };
            try {
                var http = HttpHelper.Instance;
                switch (requestType) {
                    case RequestType.Get:
                        response = await http.GetAsync(requestUrl, headers);
                        break;

                    case RequestType.Post:
                        response = await http.PostAsync(requestUrl, requestParameters, headers);
                        break;

                    case RequestType.Put:
                        response = await http.PutAsync(requestUrl, requestParameters, headers);
                        break;

                    case RequestType.Delete:
                        response = await http.DeleteAsync(requestUrl, headers);
                        break;
                }
                var result = await response.Content.ReadAsStringAsync();
                stopwatch.Stop(); //  停止监视
                var seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数
                loginfo.EndTime = DateTime.Now;
                loginfo.Seconds = seconds;
                loginfo.Result = result;
                if (!response.IsSuccessStatusCode) {
                    loginfo.ErrorMsg = string.IsNullOrEmpty(result) ? "有可能是请求头或请求参数造成的问题" : result;
                    loginfo.Level = "错误";
                } else {
                    try {
                        //这里需要和请求方约定好返回结果约定为HttpResult模型
                        var httpResult = JsonConvert.DeserializeObject<HttpResult>(result);
                        if (!httpResult.Status) {
                            loginfo.ErrorMsg = httpResult.Message;
                            loginfo.Level = "错误";
                        }
                    } catch (Exception e) {
                        loginfo.ErrorMsg = e.Message;
                        loginfo.Level = "错误";
                    }
                }
            } catch (Exception ex) {
                stopwatch.Stop(); //  停止监视
                var seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数
                loginfo.ErrorMsg = $"{ex.Message} {ex.StackTrace}";
                loginfo.Seconds = seconds;
                loginfo.Level = "错误";
            } finally {
                var seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数
                if (seconds >= warnTime)//如果请求超过20秒，记录警告日志
                {
                    loginfo.Level = "警告";
                    loginfo.ErrorMsg = $"耗时过长 - {seconds}s";
                }
                await WriteLog(loginfo);
            }
        }

        private async Task WriteLog(JobLog info) {
            var log = new JobLog {
                BeginTime = info.BeginTime,
                EndTime = info.EndTime,
                JobName = info.JobName,
                Seconds = info.Seconds,
                Result = info.Result.Length > 1999 ? info.Result.Substring(0, 1999) : info.Result,
                ErrorMsg = info.ErrorMsg,
                Level = info.Level
            };
            //知识点：DbContext实例不能在OnConfiguring中使用。原因：任务随系统启动时，是从管道中（Configure）启动的，这里会使OnConfiguring创建第二个DbContext，从而报错
            //注意此处一定要在IOCJobFactory中创建scoped作用域，因为DbContex的作用域是scoped，否则将无法解析
            var unitOfWork = _serviceProvider.GetService(typeof(IAdmUnitOfWork)) as IAdmUnitOfWork;
            var dbContext = unitOfWork.GetDbContext() as AdmDbContext;
            dbContext.JobLogs.Add(log);
            await dbContext.SaveChangesAsync();

            //输出log到控制台
            if (log.Level == "错误") {
                var msg = string.Format("--------------【{0} Job执行出错】-------------- \r\n开始时间：{1}\r\n结束时间：{2}\r\n耗    时：{3}\r\n错误消息：\r\n{4}\r\n -------------------------------------------",
                 log.JobName, log.BeginTime, log.EndTime, log.Seconds, log.ErrorMsg);
                msg.WriteErrorLine();
                //发送邮件
                var mailSetting = dbContext.MailSettings.AsQueryable().FirstOrDefault();
                if (mailSetting != null && mailSetting.Notify) {
                    var config = new MailConfig {
                        Code = mailSetting.Code,
                        Fr = mailSetting.Fr,
                        FrHost = mailSetting.FrHost,
                        ToArry = mailSetting.To.Trim().Split(','),
                        CcArray = string.IsNullOrEmpty(mailSetting.Cc) ? Array.Empty<string>() : mailSetting.Cc.Trim().Split(',')
                    };
                    var mail = new MailModel {
                        Title = $"【Job[{log.JobName}]执行出错】",
                        Body = msg
                    };
                    EmailService.PostEmail(config, mail);
                }
            } else {
                string.Format("--------------【{0} 执行成功】-------------- \r\n开始时间：{1}\r\n结束时间：{2}\r\n耗    时：{3} \r\n----------------------------------------------",
                    log.JobName, log.BeginTime, log.EndTime, log.Seconds).WriteSuccessLine();
            }
        }
    }

    /// <summary>
    /// Job任务结果
    /// </summary>
    public class HttpResult {

        /// <summary>
        /// 请求是否成功
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 异常消息
        /// </summary>
        public string Message { get; set; }
    }
}
