using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using AdmBoots.Infrastructure;
using AdmBoots.Infrastructure.Config;
using AdmBoots.Quartz;
using AdmBoots.Quartz.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl.AdoJobStore.Common;
using Quartz.Spi;

namespace AdmBoots.Api.Extensions {

    public static class QuartzStartup {

        public static void AddQuartzStartup(this IServiceCollection services) {
            if (services == null) throw new ArgumentNullException(nameof(services));
            NameValueCollection props = new NameValueCollection
            {
                { "quartz.serializer.type", "json" },
                //以下配置需要数据库表配合使用，表结构sql地址：https://github.com/quartznet/quartznet/tree/master/database/tables
                { "quartz.jobStore.type","Quartz.Impl.AdoJobStore.JobStoreTX, Quartz"},
                { "quartz.jobStore.driverDelegateType",typeof(MySQLDelegate).AssemblyQualifiedName},
                { "quartz.jobStore.tablePrefix","QRTZ_"},
                { "quartz.jobStore.dataSource","myDS"},
                { "quartz.dataSource.myDS.connectionString", DatabaseConfig.ConnectionString },
                { "quartz.dataSource.myDS.provider","MySql"},
                { "quartz.jobStore.useProperties","true"}
            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);
            services.AddSingleton<ISchedulerFactory>(factory);
            services.AddSingleton<ISchedulerCenter, SchedulerCenter>();
            services.AddSingleton<IJobFactory, IOCJobFactory>();
            services.AddTransient<HttpJob>();
        }
    }
}
