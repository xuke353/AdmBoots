using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AdmBoots.Data.EntityFrameworkCore;
using AdmBoots.Domain.Models;
using AdmBoots.Infrastructure;
using AdmBoots.Infrastructure.Extensions;
using AdmBoots.Infrastructure.Framework.Abstractions;
using AdmBoots.Quartz.Dto;
using Newtonsoft.Json;
using Quartz;

namespace AdmBoots.Quartz.Common {

    //不允许并发执行
    [DisallowConcurrentExecution]
    //在成功执行 execute() 方法后(没有抛出异常)更新 JobDetail 的 JobDataMap
    [PersistJobDataAfterExecution]
    public class HttpJob : IJob {

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
                BeginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
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
                loginfo.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                loginfo.Seconds = seconds;
                //loginfo.Result = result;
                if (!response.IsSuccessStatusCode) {
                    loginfo.ErrorMsg = result;
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
                Result = string.IsNullOrEmpty(info.ErrorMsg) ? "执行成功" : string.Empty,
                ErrorMsg = info.ErrorMsg,
                Level = info.Level
            };
            //输出log到控制台
            if (string.IsNullOrEmpty(log.Result)) {
                $"--------------【{log.JobName} 出错】--------------".WriteErrorLine();
                log.ErrorMsg.WriteErrorLine();
                "--------------------------------------------------".WriteErrorLine();
            } else {
                $"--------------【{log.JobName} 执行成功】--------------".WriteSuccessLine();
                $"开始时间：{log.BeginTime}".WriteInfoLine();
                $"结束时间：{log.EndTime}".WriteInfoLine();
                $"耗    时：{log.Seconds}秒".WriteInfoLine();
                "-----------------------------------------------------".WriteSuccessLine();
            }
            var dbContext = AdmBootsApp.ServiceProvider.GetService(typeof(AdmDbContext)) as AdmDbContext;
            dbContext.JobLogs.Add(log);
            await dbContext.SaveChangesAsync();
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
