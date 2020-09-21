<h1 align="center">AdmBoots</h1>
<h3 align="center">:lemon: :tangerine: :cherries: :cake: :grapes: :watermelon: :strawberry: :corn: :peach: :melon:</h3>
<p align="center">
  <img src="https://img.shields.io/badge/license-MIT-brightgreen.svg">
</p>
基于.NET Core3.1 + EF Core3.1最成熟技术体系，借鉴众多开源项目的优点，诣在打造前后端分离快速开发框架，让你开发Web管理系统和移动端Api更简单。

## 系统结构
![](https://images.cnblogs.com/cnblogs_com/xuke/1851804/o_200921133354框架结构图.jpg)
![](https://gitee.com/xuke353/AdminSoa/raw/file/框架结构图.jpg)
若图片不显示请点击 [这里](https://images.cnblogs.com/cnblogs_com/xuke/1851804/o_200921133354框架结构图.jpg)
## 技术栈
- **DDD领域驱动思想**
- **EF Core3.1**  超全的仓储层操作封装
- **Swagger接口文档** 支持RESTful
- **AutoMapper DTO映射**
- **工作单元实现** 自动事务处理
- **AutoFac** 依赖注入/拦截器
- **HealthChecks健康监控**
- **分布式缓存** Memory/Redis
- **Serilog记录日志**
- **全局异常处理**
- **基于JWT策略授权** 实现通用权限管理功能：用户-角色-权限（菜单/按钮）
- **Quartz.net作业调度**
## 工程结构
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
