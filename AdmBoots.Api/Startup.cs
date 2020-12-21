using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AdmBoots.Application;
using AdmBoots.Data.EntityFrameworkCore;
using AdmBoots.Domain.IRepositories;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using AdmBoots.Api.Filters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AdmBoots.Infrastructure.Config;
using AdmBoots.Data.EntityFrameworkCore.Uow;
using Microsoft.Extensions.Logging;
using AdmBoots.Infrastructure.Framework.Web;
using AdmBoots.Api.Extensions;
using AdmBoots.Infrastructure.SignalR;
using AdmBoots.Quartz;
using AdmBoots.Data.EntityFrameworkCore.Seed;
using AdmBoots.Infrastructure.Ioc;
using AdmBoots.Infrastructure.Auditing;
using AspNetCoreRateLimit;
using AdmBoots.Api.Middleware;

namespace AdmBoots.Api {

    public class Startup {
        public static readonly ILoggerFactory EFLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env, IConfiguration configuration) {
            Environment = env;
            Configuration = configuration;
            DatabaseConfig.InitConfiguration(configuration);
        }

        /// <summary>
        /// AutoFac容器
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder) {
            //注册拦截器
            builder.RegisterType<UnitOfWorkInterceptor>().AsSelf();
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            //不能用单例
            builder.RegisterType<AdmSession>().As<IAdmSession>();
            builder.RegisterType<HttpContextClientInfoProvider>().As<IClientInfoProvider>();

            #region Application层注入

            //AdmBoots.Application是继承接口的实现方法类库名称
            var assemblys = Assembly.Load("AdmBoots.Application");
            //ITransientDependency 是一个接口（所有Application层要实现依赖注入的接口都要继承该接口）
            var baseType = typeof(ITransientDependency);
            builder.RegisterAssemblyTypes(assemblys)
                .Where(m => baseType.IsAssignableFrom(m) && m != baseType && !m.IsAbstract)
            .AsImplementedInterfaces()
            .PropertiesAutowired()                       //支持属性注入
            .EnableInterfaceInterceptors()               //启用接口拦截
            .InterceptedBy(typeof(UnitOfWorkInterceptor));

            #endregion Application层注入

            #region Data层注入

            //Data层实现接口的类自动依赖注入
            var basePath = AppContext.BaseDirectory;
            var repositoryDllFile = Path.Combine(basePath, "AdmBoots.Data.dll");
            var assemblysRepository = Assembly.LoadFrom(repositoryDllFile);
            builder.RegisterAssemblyTypes(assemblysRepository)
                .AsImplementedInterfaces();

            #endregion Data层注入

            builder.RegisterGeneric(typeof(AdmRepositoryBase<,>)).As(typeof(IRepository<,>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(AdmRepositoryBase<>)).As(typeof(IRepository<>)).InstancePerDependency();
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddSwaggerSetup(Configuration);
            services.AddCacheSetup(Configuration);
            services.AddAuthorizationSetup(Configuration);
            services.AddHealthChecksSetup(Configuration);
            services.AddIpRateLimitSetup(Configuration);
            services.AddQuartzStartup();
            services.AddAutoMapper(Assembly.Load("AdmBoots.Application"));
            services.AddApiVersioning(option => option.ReportApiVersions = true);
            services.AddDbContext<AdmDbContext>(option => {
                option.UseMySql(DatabaseConfig.ConnectionString, new MySqlServerVersion(new Version(DatabaseConfig.Version)));
                if (Environment.IsDevelopment()) {
                    //打印sql
                    option.UseLoggerFactory(EFLoggerFactory);
                    option.EnableSensitiveDataLogging(true);//显示sql参数
                }
            });

            services.AddControllers(option => {
                option.Filters.Add(typeof(GlobalExceptionFilter));
                option.Filters.Add(typeof(AuditActionFilter));
            }).AddNewtonsoftJson(option => {
                //忽略循环引用
                option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            //Enables the JSON protocol for SignalR
            services.AddSignalR().AddNewtonsoftJsonProtocol();

            services.AddMiniProfiler(options => {
                options.RouteBasePath = "/profiler";
            }).AddEntityFramework();

            services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder => {
                builder.SetIsOriginAllowed(origin => true)//允许所有 origin 来源
                .AllowAnyMethod()
                .AllowAnyHeader()
                //.WithOrigins(Configuration["Startup:Cors"].Split(',')) //允许特定来源
                .AllowCredentials();
            }));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider) {
            IocManager.SetProvider(new IocFactory(app.ApplicationServices));
            //开启任务调度
            if (Configuration.GetValue<bool>("Startup:Scheduler")) {
                app.ApplicationServices.GetService<ISchedulerCenter>().Start();
            }

            //程序第一次运行时初始化数据库及种子数据, 初始化完毕后，可注销此处避免每次都做数据库检查
            app.InitializeDatabase();
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                Console.WriteLine("任务调度：" + (Configuration.GetValue<bool>("Startup:Scheduler") ? "启动" : "关闭"));
                Console.WriteLine("实时通讯：" + (Configuration.GetValue<bool>("Startup:SignalR") ? "启用" : "关闭"));
                Console.WriteLine("健康检查：" + (Configuration.GetValue<bool>("Startup:HealthChecks") ? "启用" : "关闭"));
                Console.WriteLine("IP限流：" + (Configuration.GetValue<bool>("Startup:IpRateLimit") ? "启用" : "关闭"));
            }
            //↓↓↓↓注意以下中间件顺序↓↓↓↓

            #region Swagger

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                foreach (var description in provider.ApiVersionDescriptions) {
                    c.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
                c.IndexStream = () => GetType().GetTypeInfo().Assembly
                   .GetManifestResourceStream("AdmBoots.Api.wwwroot.swagger.ui.index.html");
                c.RoutePrefix = string.Empty;//设置为空，launchSettings.json把launchUrl去掉,localhost:8082 代替 localhost:8002/swagger
            });

            #endregion Swagger

            app.UseStaticFiles();
            app.UseRouting();
            //认证
            app.UseAuthentication();
            //授权
            app.UseAuthorization();
            //跨域
            app.UseCors("CorsPolicy");
            //MiniProfiler
            app.UseMiniProfiler();

            if (Configuration.GetValue("Startup:IpRateLimit", false)) {
                //限流
                //app.UseIpRateLimiting();
                app.UseMiddleware<IPLimitMiddleware>();
            }

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.UseHealthChecks();
                endpoints.MapHub<ChatHub>("/api/chatHub");
            });
        }
    }
}
