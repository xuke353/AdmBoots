using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace AdmBoots.Quartz.Common {
    public class IOCJobFactory : IJobFactory {
        private readonly IServiceProvider _serviceProvider;
        public IOCJobFactory(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler) {
            //这里不能使用using var scope = _serviceProvider.CreateScope();
            //原因：此处的作用域释放后，HttpJob中无法解析IAdmUnitOfWork实例
            var scope = _serviceProvider.CreateScope();
            var job = scope.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
            return job;
        }

        public void ReturnJob(IJob job) {
            (job as IDisposable)?.Dispose();
        }
    }
}
