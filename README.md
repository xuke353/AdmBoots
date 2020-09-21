<h1 align="center">DVA Boot Admin</h1>
<h3 align="center">:lemon: :tangerine: :cherries: :cake: :grapes: :watermelon: :strawberry: :corn: :peach: :melon:</h3>

基于.netcore3.1快速开发框架

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
