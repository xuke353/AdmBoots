AdmBoots 基于.NET Core3.1 + EF Core3.1最成熟技术体系，借鉴众多开源项目的优点，诣在打造前后端分离快速开发框架，让你开发Web管理系统和移动端Api更简单。
> 群号：877617006
## 系统结构
系统采用前后端分离模式，前端使用开箱即用的开源脚手架[adv-boot-admin](https://github.com/LANIF-UI/dva-boot-admin)，本篇将重点介绍后端技术体系及使用方法。整体架构如下图

![AdmBoots.jpg](https://i.loli.net/2020/09/22/JnrW6ONts2mV9hA.jpg)

## 技术栈
AdmBoots目前使用技术栈如下，当然这只是.Net 技术体系的冰山一角，后期可能会增加其他技术，比如signalR，简单消息队列等。
- **.Net Core 3.1**
- **EF Core3.1**  超全的仓储操作封装
- **DDD领域驱动思想**
- **Swagger接口文档** 支持RESTful
- **AutoMapper DTO映射**
- **工作单元实现** 自动事务处理
- **AutoFac** 轻量级IoC和DI依赖注入，支持属性注入
- **简单的跨域解决方案**
- **AOP**
- **HealthChecks健康监控**
- **分布式缓存** Memory/Redis
- **Serilog记录日志**
- **全局异常处理**
- **基于JWT策略授权** 实现通用权限管理功能：用户-角色-权限（菜单/按钮）
- **Quartz.net作业调度** 支持发送邮件
  
## 实现功能
AdmBoots实现了实现通用权限管理，有了这些基础功能，只需要往里添加一些业务功能就可实现一个 后台管理系统。为简化CURD功能的开发，框架配备了代码是生成器。
- **菜单管理**
- **角色管理**
- **任务调度**

## 工程结构
采用DDD领域驱动设计，其中任务调度单独一层

![项目分层.jpg](https://i.loli.net/2020/09/26/sYTRjxXgaJNdzom.jpg)


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
AdmBoots保证了最小系统环境要求，具备以下工具即可运行本系统

* Visual Studio 2019 必须
* MySQL-5.7          必须
* Redis              非必须

### 运行系统
1. 首先在GitHub上将代码Clone下来后，用VS2019打开，第一次打开项目工具会自动安装系统依赖的Nuget包，静静等待即可。如果Nuget包还原失败，可以在解决方案上右键【还原Nuget包】
2. 打开API层的 <b>appsetting.json</b> 文件，设置系统启动的必要配置
  
  a.数据库配置 -必须
 ```
//数据库配置
 "Database": {
    "DbProviderName": "MySql", //只能写"Sqlite","SqlServer","MySql","Oracle"其中一个,并修改对应的连接字符串
    "ConnectionString": "Server=localhost; Port=3306;Stmt=; Database=admboot; Uid=root; Pwd=root;"
    //"SqlServerConnection": "Server=.;Database=WMBlogDB;User ID=sa;Password=123;",
    //"OracleConnection": "Provider=OraOLEDB.Oracle; Data Source=WMBlogDB; User Id=sss; Password=789;",
    //"SqliteConnection": "DataSource=C:\\WMBlog.db"
  }

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
   初始化种子数据分为两种方式
   
   a. sql脚本插入 <b>SQL</b> 文件夹，打开项目根目录下的，将里面的 AdmBoots_mysql.sql，quartz_mysql.sql 文件插入到数据库
   
   b. 使用Code First迁移命令，这里已为大家生成好迁移文件，打开程序包管理控制台，大家只需要做执行以下命令即可
    
    ```
    PM> update-database -Context AdmDbContext
    ```
至此，我们的准备工作全部做完了，启动项目，出现Swagger页面，项目初始化成功！

![swagger.jpg](https://i.loli.net/2020/09/26/UXWdTZv4wJ1MqAl.jpg)
## 最后
asp.net core的默认（csproj里面没有设置的情况下），是进程外启动，所以这里推荐使用项目启动方式，项目启动本质上就是执行了 dotnet run AdmBoots.Api.dll。不建议大家更改进程内启动(InProcess)，因为InProcess方式在我们发布项目后，可能会出现一些路径上的问题（相对了托管服务器的路径）

![服务器.jpg](https://i.loli.net/2020/09/26/KRhAIu8BQn7vZOX.jpg)

```
<PropertyGroup>
     <TargetFramework>netcoreapp2.2</TargetFramework>
     <AspNetCoreHostingModel>InProcess</AspNetCoreHostingModel>
</PropertyGroup>
 ```
> 当配置了OutOfProcess或者不配置的时候，无论使用iis启动还是dotnet run启动都是外进程，使用iis启动可以理解为iis作为了反向代理，用dotnet run启动可以视为自己作为内部服务器和反向代理结合使用


 下一节我们讲如何 使用该框架进行业务开发，以及权限配置使用，平时比较忙，更新可能不及时，有问题的小伙伴可以先入群

## 源码地址
后端：<https://github.com/xuke353/AdmBoots>

前端：<https://github.com/xuke353/AdmBoots-Client>

## 实践
[.NetCore3.1前后端分离快速开发框架：实践](https://www.cnblogs.com/xuke/p/13736724.html)
