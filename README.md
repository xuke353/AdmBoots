<img style="display: block; margin: 0 auto;" src="https://i.loli.net/2020/12/14/4RfJq7tSGKkH8QU.png" alt="" />

详情-> https://www.cnblogs.com/xuke/p/13573837.html

## 序言

AdmBoots 基于.NET Core生态系统最成熟技术体系，借鉴众多开源项目的优点，旨在打造前后端分离快速开发框架，让你开发Web管理系统和移动端Api更简单。

框架对依赖注入、日志、缓存、模型映射、认证/授权、仓储、健康检测、ORM，任务调度等模块进行更高一级的自动化封装。

📌**项目特点**：代码简洁、易上手、学习成本低、开箱即用

📌**适用人群**：.NetCore入门同学，正在寻找具有以上特点框架的同学

## 系列教程

[*01*.NetCore(.Net5)快速开发框架一：前言](https://www.cnblogs.com/xuke/p/13573837.html)

[*02*.NetCore(.Net5)快速开发框架二：快速开发](https://www.cnblogs.com/xuke/p/13736724.html)

[*03*.NetCore(.Net5)快速开发框架三：WebAPI性能监控-MiniProfiler与Swagger集成](https://www.cnblogs.com/xuke/p/13847248.html)

[*04*.NetCore(.Net5)快速开发框架四：实现审计日志](https://www.cnblogs.com/xuke/p/14070985.html)

...

## 架构设计

正在整理中...

## 功能与技术

采用前后端分离模式，后端基于.Net5平台![后端](https://i.loli.net/2020/12/15/qfD2OhY89icvumH.png)

前端使用基于React的开源脚手架[adv-boot-admin](https://github.com/LANIF-UI/dva-boot-admin)

![](https://i.loli.net/2020/12/15/cdAi8x7jJuSyRvN.png)



未来要实现的功能：

- 上两图中未完成的功能（图中未打对号的）
- 持续部署集成
- 重构完善
- 消息队列
- ...

## 工程结构

采用DDD领域驱动设计，经典分层结构，系统层次少，分层清晰，对常翻代码的开发人员十分友好。

![](https://i.loli.net/2020/12/16/m4OYyQGkWrH1g2R.png)

如果我们我们的业务是单领域的，那么只需要在Domain层创建model，在Data层写上一句DbSet<实体名>，在Application层写上我们的逻辑，在API层公开出去，一个简单的功能开发就完成了。配上代码生成器，我们的编码就更少了，只注重业务本身的开发就可以了。

![](https://img2020.cnblogs.com/blog/591405/202009/591405-20200926113414883-1093892868.jpg)

![关系布局图](https://i.loli.net/2020/12/15/8KeSf1VYEWxCHm6.jpg)


```
.
├── AdmBoots.Api                # API接口层
|   ├── Authorization           # 策略授权认证相关
|   ├── Controllers             # 控制器，对外提供（Restful）接口
|   ├── Extensions              # ConfigureServices扩展
|   ├── Filters                 # 过滤器
├── AdmBoots.Application        # 应用层
|   ├── AutoMapProfile.cs       # AutoMapper 映射文件
|   ├── AppServiceBase.cs       # service基类，提供一些常用属性
|   ├── ITransientDependency.cs # 依赖注入继承接口，继承此接口可将Application层注入到容器
├── AdmBoots.Data               # 基础设施层-持久化层
|   ├── EntityFrameworkCore     # EF Core相关
│   │   ├── Repositories        # 自定义仓储写在这里  
│   │   │   ├── SqlExecuterRepository.cs# 通用sql执行器
│   │   ├── Seed                # 种子数据
│   │   └── Uow                 # 工作单元(事务处理)
│   │   └── AdmDbContext.cs     # 数据库上下文 
│   │   └── AdmRepositoryBase.cs# 仓储基类，在大多数的时候，这些方法已足够应付一般实体的需要      
├── AdmBoots.Domain             # 领域层
|   ├── IRepositories           # 仓储接口
|   ├── Models                  # 模型
├── AdmBoots.Infrastructure     # 基础设施层-通用技术支持
├── AdmBoots.Quartz             # 基于Quartz.NET的任务调度
```

## 项目初始化

### 环境要求

* Visual Studio 2019 16.8 +
* .NET 5 SDK +
* MySQL （支持其他库，详见[EFCore](https://docs.microsoft.com/en-us/ef/core/providers/?tabs=dotnet-core-cli)）
* Redise (非必须)

### 运行系统

#### 初始化服务端

1. 首先在GitHub上将代码Clone下来后，用VS2019打开，第一次打开项目工具会自动安装系统依赖的Nuget包，静静等待即可。如果Nuget包还原失败，可以在解决方案上右键【还原Nuget包】
2. 打开API层的 <b>appsetting.json</b> 文件，设置系统启动的必要配置

  a.数据库配置 -必须

 ```
 //数据库连接字符串
  "ConnectionStrings": {
    "Default": "Server=localhost; Port=3306;Stmt=; Database=admboot; Uid=root; Pwd=root;"
  },
  //数据库类型
  "Database": {
    "Dialect": "mysql",
    "Version": "8.0.22" //.Net5 MySQL连接驱动要求提供版本号 https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql
  },

 ```

 b.缓存配置 -非必须，如需使用Redis，请配置Redis连接。默认MemoryCache

 ```
//Redis配置 Configuration=""时，系统使用MemoryCache
 "Redis": {
    "Configuration": "", //127.0.0.1:6379
    "InstanceName": "AdmBoots"
  },
 ```

 c.日志配置 -非必须，如果需要将日志输出到数据库，在Serilog节点需配置对应数据库连接。默认输出到文件

 ```
  //Serilog配置
  "Serilog":
    "WriteTo": [
      {
        "Name": "MySQL",
        "Args": {
          "connectionString": "Server=localhost; Port=3306;Stmt=; Database=admboot; Uid=root; Pwd=root;",
          "tableName": "Logs",
         "restrictedToMinimumLevel": "Warning"
        }
      }     
    ]
  }
 ```

3. 初始化种子数据
   初始化种子数据分为三种方式
   a.第一次运行程序会自动初始化种子数据。

   ```c#
     public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider) {
               ...
               //程序第一次运行时初始化数据库及种子数据, 初始化完毕后，可注销此处避免每次都做数据库检查
               app.InitializeDatabase();
   			...
               }
   ```

   b. 使用Code First迁移命令，这里已为大家生成好迁移文件，打开程序包管理控制台，大家只需要做执行以下命令即可

   ```
    PM> update-database -Context AdmDbContext
   ```

   c. sql脚本插入 <b>SQL</b> 文件夹，打开项目根目录下的，将里面的 AdmBoots_mysql.sql

   使用**Quartz.Net定时任务**功能需手动执行sql文件quartz_mysql.sql 
   （其他数据库建表脚本https://github.com/quartznet/quartznet/tree/master/database/tables）

4. 验证

   启动项目，出现Swagger页面，项目初始化成功！

   Swagger页面上，如果Controller上有小锁的标志，表示需要先登录后才能进行测试验证。点击authorize按钮，输入admin，a123456登陆。

   #### ![](https://img2020.cnblogs.com/blog/591405/202009/591405-20200926101742628-2049378554.jpg)

   #### 初始化前端

   ```
   $ git clone https://github.com/xuke353/AdmBoots-Client.git
   $ cd AdmBoots-Client
   // 安装依赖
   $ yarn
   // 启动
   $ yarn start
   ```

   启动成功后（先保证服务端开启），浏览器输入http://localhost:3000/

   #### ![](https://i.loli.net/2020/12/16/9hmpoCM3yjQgPex.jpg)

![](https://i.loli.net/2020/12/16/7jeaAGmfDutSWzO.jpg)

![](https://i.loli.net/2020/12/16/oqBilpKTvzObLA3.jpg)

![](https://i.loli.net/2020/12/16/PR2nUiYa5fQ7S9r.jpg)

![](https://i.loli.net/2020/12/16/IDYPihqj1Lux2SH.jpg)

## 最后

asp.net core的默认（csproj里面没有设置的情况下），是进程外启动，所以这里推荐使用项目启动方式，项目启动本质上就是执行了 dotnet run AdmBoots.Api.dll。不建议大家更改进程内启动(InProcess)，因为InProcess方式在我们发布项目后，可能会出现一些路径上的问题（相对了托管服务器的路径）

![](https://img2020.cnblogs.com/blog/591405/202009/591405-20200926102620618-2128066691.jpg)

```
<!--AdmBoots.Api.csproj 使用进程内启动（不建议）-->
<PropertyGroup>
     <TargetFramework>netcoreapp2.2</TargetFramework>
     <AspNetCoreHostingModel>InProcess</AspNetCoreHostingModel>
</PropertyGroup>
```

> 当配置了OutOfProcess或者不配置的时候，无论使用iis启动还是dotnet run启动都是外进程，使用iis启动可以理解为iis作为了反向代理，用dotnet run启动可以视为自己作为内部服务器和反向代理结合使用


 下一篇我们讲如何 使用该框架进行业务开发，以及权限配置使用，平时比较忙，更新可能不及时，有问题的小伙伴可以先入群

## 源码地址

后端：<https://github.com/xuke353/AdmBoots>

前端：<https://github.com/xuke353/AdmBoots-Client>
