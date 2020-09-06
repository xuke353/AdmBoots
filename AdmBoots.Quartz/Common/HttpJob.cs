using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
            var maxLogCount = 20;//最多保存日志数量
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

            var loginfo = new LogInfo {
                BeginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                JobName = $"{context.JobDetail.Key.Group}.{context.JobDetail.Key.Name}",
            };

            var logs = context.JobDetail.JobDataMap[QuartzConstant.LOGLIST] as List<string> ?? new List<string>();
            if (logs.Count >= maxLogCount)
                logs.RemoveRange(0, logs.Count - maxLogCount);

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
                //var result = HttpUtility.HtmlEncode(await response.Content.ReadAsStringAsync());
                var result = await response.Content.ReadAsStringAsync();
                stopwatch.Stop(); //  停止监视            
                var seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数                                
                loginfo.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                loginfo.Seconds = seconds;
                loginfo.Result = result;
                if (!response.IsSuccessStatusCode) {
                    loginfo.ErrorMsg = result;
                    loginfo.Level = "错误";
                    context.JobDetail.JobDataMap[QuartzConstant.EXCEPTION] = HtmlSerializeObject(loginfo);
                } else {
                    try {
                        //这里需要和请求方约定好返回结果约定为HttpResult模型
                        var httpResult = JsonConvert.DeserializeObject<ResponseBody<object>>(result);
                        if (!httpResult.Status) {
                            loginfo.ErrorMsg = httpResult.Message;
                            context.JobDetail.JobDataMap[QuartzConstant.EXCEPTION] = HtmlSerializeObject(loginfo);
                        }
                    } catch (Exception e) {
                        loginfo.ErrorMsg = e.Message;
                        context.JobDetail.JobDataMap[QuartzConstant.EXCEPTION] = HtmlSerializeObject(loginfo);
                    }
                }
            } catch (Exception ex) {
                stopwatch.Stop(); //  停止监视            
                var seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数
                loginfo.ErrorMsg = $"{ex.Message} {ex.StackTrace}";
                context.JobDetail.JobDataMap[QuartzConstant.EXCEPTION] = HtmlSerializeObject(loginfo);
                loginfo.Seconds = seconds;
            } finally {
                logs.Add($"<p>{HtmlSerializeObject(loginfo)}</p>");
                context.JobDetail.JobDataMap[QuartzConstant.LOGLIST] = logs;
                var seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数
                if (seconds >= warnTime)//如果请求超过20秒，记录警告日志    
                {
                    loginfo.ErrorMsg = $"耗时过长 - {seconds}s";
                }
            }
        }

        private string HtmlSerializeObject(LogInfo info) {
            var result = HttpUtility.HtmlEncode(info.Result);
            var logInfo = new LogInfo {
                BeginTime = info.BeginTime,
                EndTime = info.EndTime,
                JobName = info.JobName,
                Seconds = info.Seconds,
                Result = $"<span class='result'>{result.PadLeft(1000)}</span>",
                ErrorMsg = $"<span class='error'>{info.ErrorMsg}</span>"
            };
            return JsonConvert.SerializeObject(logInfo);
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
