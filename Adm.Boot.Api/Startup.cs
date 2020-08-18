using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Adm.Boot.Application;
using Adm.Boot.Data.EntityFrameworkCore;
using Adm.Boot.Domain.IRepositories;
using Adm.Boot.Infrastructure;
using Adm.Boot.Infrastructure.Interceptors;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Adm.Boot.Infrastructure.Extensions;
using Adm.Boot.Api.StartupExtensions;
using Newtonsoft.Json;
using Adm.Boot.Api.Filters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Adm.Boot.Infrastructure.Config;

namespace Adm.Boot.Api {
    public class Startup {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env) {
            //如果配置文件根据环境变量来分开了，可以这样写
            //Path = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json";
            Configuration = new ConfigurationBuilder()
             .SetBasePath(env.ContentRootPath)
            //ReloadOnChange = true 当appsettings.json被修改时重新加载
            .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
            .Build();
            AdmApp.Configuration = Configuration;
        }

        /// <summary>
        /// AutoFac容器
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder) {
            //注册拦截器
            // builder.RegisterType<TransactionInterceptor>().AsSelf();
            // builder.RegisterType<TransactionAsyncInterceptor>().AsSelf();
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            //builder.RegisterType<AdminSession>().As<IAdminSession>();
            try {
                //Adm.Boot.Application是继承接口的实现方法类库名称
                var assemblys = Assembly.Load("Adm.Boot.Application");
                //ITransientDependency 是一个接口（所有Application层要实现依赖注入的借口都要继承该接口）
                var baseType = typeof(ITransientDependency);
                builder.RegisterAssemblyTypes(assemblys)
                    .Where(m => baseType.IsAssignableFrom(m) && m != baseType && !m.IsAbstract)
                .AsImplementedInterfaces()
                .PropertiesAutowired();                      //支持属性注入
                //.EnableInterfaceInterceptors()               //启用接口拦截
                //.InterceptedBy(typeof(TransactionInterceptor));

                var basePath = AppContext.BaseDirectory;
                var repositoryDllFile = Path.Combine(basePath, "Adm.Boot.Data.dll");
                var assemblysRepository = Assembly.LoadFrom(repositoryDllFile);
                builder.RegisterAssemblyTypes(assemblysRepository)
                    .AsImplementedInterfaces();

                builder.RegisterGeneric(typeof(AdmRepositoryBase<,>)).As(typeof(IRepository<,>)).InstancePerDependency();
            } catch (Exception ex) {
                ("Adm.Boot.Data.dll 丢失，请先编译再运行。\n" + ex.Message).WriteErrorLine();
                throw;
            }
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddSwaggerSetup();
            services.AddAutoMapper(Assembly.Load("Adm.Boot.Application"));
            services.AddApiVersioning(option => option.ReportApiVersions = true);
            services.AddDbContext<AdmDbContext>(option => option.UseMySql(DatabaseConfig.ConnectionString));
            services.AddControllers(o => {
                o.Filters.Add(typeof(GlobalExceptionFilter));
            }).AddNewtonsoftJson(options => {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider) {
            AdmApp.ServiceProvider = app.ApplicationServices;

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            //↓↓↓↓注意以下中间件顺序↓↓↓↓

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                foreach (var description in provider.ApiVersionDescriptions) {
                    c.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
                //c.IndexStream = () => Assembly.GetExecutingAssembly()
                //   .GetManifestResourceStream("Adm.Boot.Api.wwwroot.swagger.index.html");
                c.RoutePrefix = "";//设置为空，launchSettings.json把launchUrl去掉,localhost:8081 代替 localhost:8001/swagger               
            });
            #endregion
        }
    }
}
