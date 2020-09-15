using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Domain;
using AdmBoots.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AdmBoots.Data.EntityFrameworkCore.Seed {
    /// <summary>
    /// 系统初始化时生成种子数据
    /// </summary>
    public static class SeedData {
        public static void GenerateSeedData(this ModelBuilder modelBuilder) {
            //不在这里设置默认值，会报错
            modelBuilder.Entity<User>().Property(p => p.IsMaster).HasDefaultValue(false);
            modelBuilder.Entity<User>().HasData(
                //user
                new User {
                    Id = 1,
                    Name = "管理员",
                    UserName = "admin",
                    Password = "DC483E80A7A0BD9EF71D8CF973673924",//a123456
                    IsMaster = true,
                    CreateTime = DateTime.Now,
                    Status = SysStatus.有效
                },
                new User {
                    Id = 2,
                    Name = "张无忌",
                    UserName = "zhangwj",
                    Password = "E10ADC3949BA59ABBE56E057F20F883E",//123456
                    IsMaster = false,
                    CreateTime = DateTime.Now,
                    Status = SysStatus.有效
                },
                new User {
                    Id = 3,
                    Name = "周芷若",
                    UserName = "zhouzr",
                    Password = "E10ADC3949BA59ABBE56E057F20F883E",//123456
                    IsMaster = false,
                    CreateTime = DateTime.Now,
                    Status = SysStatus.有效
                });
            modelBuilder.Entity<Role>().HasData(
            //role
            new Role {
                Id = 1,
                Name = "系统管理员",
                Code = "xtgly",
                Status = SysStatus.有效,
                CreateTime = DateTime.Now,
                CreatorId = 1,
                CreatorName = "管理员",
                Description = "拥有最高权限"
            },
            new Role {
                Id = 2,
                Name = "掌门人",
                Code = "zmr",
                Status = SysStatus.有效,
                CreateTime = DateTime.Now,
                CreatorId = 1,
                CreatorName = "管理员",
                Description = "拥有最高权限"
            });
            modelBuilder.Entity<Menu>().HasData(
            //menu
            new Menu {
                Id = 1,
                Name = "仪表盘",
                Code = "yibp",
                Icon = "AreaChartOutlined",
                IsActive = true,
                MenuType = MenuType.菜单,
                Description = "菜单的Uri为路由地址",
                Uri = "/dashboard",
                Sort = 0,
                Status = SysStatus.有效,
                ParentId = -1,
                CreatorId = 1,
                CreatorName = "管理员",
                CreateTime = DateTime.Now,
            },
            new Menu {
                Id = 2,
                Name = "系统管理",
                Code = "xitgl",
                Icon = "ClusterOutlined",
                IsActive = true,
                MenuType = MenuType.菜单,
                Description = "菜单的Uri为路由地址",
                Uri = "/system",
                Sort = 1,
                Status = SysStatus.有效,
                ParentId = -1,
                CreatorId = 1,
                CreatorName = "管理员",
                CreateTime = DateTime.Now,
            },
            new Menu {
                Id = 3,
                Name = "任务调度",
                Code = "zuoydd",
                Icon = "ScheduleOutlined",
                IsActive = true,
                MenuType = MenuType.菜单,
                Description = "菜单的Uri为路由地址",
                Uri = "/schedule",
                Sort = 2,
                Status = SysStatus.有效,
                ParentId = -1,
                CreatorId = 1,
                CreatorName = "管理员",
                CreateTime = DateTime.Now,
            },
            new Menu {
                Id = 4,
                Name = "角色管理",
                Code = "juesgl",
                Icon = "ClusterOutlined",
                IsActive = true,
                MenuType = MenuType.菜单,
                Description = "菜单的Uri为路由地址",
                Uri = "/role",
                Sort = 1,
                Status = SysStatus.有效,
                ParentId = 2,
                CreatorId = 1,
                CreatorName = "管理员",
                CreateTime = DateTime.Now,
            },
            new Menu {
                Id = 5,
                Name = "菜单管理",
                Code = "caidgl",
                Icon = "MenuOutlined",
                IsActive = true,
                MenuType = MenuType.菜单,
                Description = "菜单的Uri为路由地址",
                Uri = "/menu",
                Sort = 2,
                Status = SysStatus.有效,
                ParentId = 2,
                CreatorId = 1,
                CreatorName = "管理员",
                CreateTime = DateTime.Now,
            },
            new Menu {
                Id = 6,
                Name = "用户管理",
                Code = "yonghugl",
                Icon = "UserSwitchOutlined",
                IsActive = true,
                MenuType = MenuType.菜单,
                Description = "菜单的Uri为路由地址",
                Uri = "/user",
                Sort = 3,
                Status = SysStatus.有效,
                ParentId = 2,
                CreatorId = 1,
                CreatorName = "管理员",
                CreateTime = DateTime.Now,
            },
            new Menu {
                Id = 7,
                Name = "任务列表",
                Code = "renwlb",
                Icon = "OrderedListOutlined",
                IsActive = true,
                MenuType = MenuType.菜单,
                Description = "菜单的Uri为路由地址",
                Uri = "/job",
                Sort = 1,
                Status = SysStatus.有效,
                ParentId = 3,
                CreatorId = 1,
                CreatorName = "管理员",
                CreateTime = DateTime.Now,
            },
            new Menu {
                Id = 8,
                Name = "邮箱设置",
                Code = "youxsz",
                Icon = "MailOutlined",
                IsActive = true,
                MenuType = MenuType.菜单,
                Description = "菜单的Uri为路由地址",
                Uri = "/mailSetting",
                Sort = 2,
                Status = SysStatus.有效,
                ParentId = 3,
                CreatorId = 1,
                CreatorName = "管理员",
                CreateTime = DateTime.Now,
            },
            new Menu {
                Id = 9,
                Name = "添加",
                Code = "add",
                IsActive = true,
                MenuType = MenuType.按钮,
                Description = "编号是前端判断权限的key",
                Uri = "Role:Add",
                Sort = 0,
                Status = SysStatus.有效,
                ParentId = 4,
                CreatorId = 1,
                CreatorName = "管理员",
                CreateTime = DateTime.Now,
            },
            new Menu {
                Id = 10,
                Name = "查询",
                Code = "query",
                IsActive = true,
                MenuType = MenuType.按钮,
                Description = "编号是前端判断权限的key",
                Uri = "Role:Query",
                Sort = 1,
                Status = SysStatus.有效,
                ParentId = 4,
                CreatorId = 1,
                CreatorName = "管理员",
                CreateTime = DateTime.Now,
            },
            new Menu {
                Id = 11,
                Name = "修改",
                Code = "update",
                IsActive = true,
                MenuType = MenuType.按钮,
                Description = "编号是前端判断权限的key",
                Uri = "Role:Update",
                Sort = 2,
                Status = SysStatus.有效,
                ParentId = 4,
                CreatorId = 1,
                CreatorName = "管理员",
                CreateTime = DateTime.Now,
            },
            new Menu {
                Id = 12,
                Name = "删除",
                Code = "delete",
                IsActive = true,
                MenuType = MenuType.按钮,
                Description = "编号是前端判断权限的key",
                Uri = "Role:Delete",
                Sort = 3,
                Status = SysStatus.有效,
                ParentId = 4,
                CreatorId = 1,
                CreatorName = "管理员",
                CreateTime = DateTime.Now,
            },
            new Menu {
                Id = 13,
                Name = "权限",
                Code = "auth",
                IsActive = true,
                MenuType = MenuType.按钮,
                Description = "编号是前端判断权限的key",
                Uri = "Role:UpdateRoleMenu",
                Sort = 4,
                Status = SysStatus.有效,
                ParentId = 4,
                CreatorId = 1,
                CreatorName = "管理员",
                CreateTime = DateTime.Now,
            });
            modelBuilder.Entity<UserRole>().HasData(
             //user_role
             new UserRole {
                 Id = 1,
                 UserId = 1,
                 RoleId = 1,
             },
             new UserRole {
                 Id = 2,
                 UserId = 2,
                 RoleId = 2,
             },
             new UserRole {
                 Id = 3,
                 UserId = 3,
                 RoleId = 2,
             });
            modelBuilder.Entity<RoleMenu>().HasData(
             //role_menu
             new RoleMenu {
                 Id = 1,
                 RoleId = 1,
                 MenuId = 1,
             },
             new RoleMenu {
                 Id = 2,
                 RoleId = 1,
                 MenuId = 2,
             },
             new RoleMenu {
                 Id = 3,
                 RoleId = 1,
                 MenuId = 3,
             },
             new RoleMenu {
                 Id = 4,
                 RoleId = 1,
                 MenuId = 4,
             },
             new RoleMenu {
                 Id = 5,
                 RoleId = 1,
                 MenuId = 5,
             },
             new RoleMenu {
                 Id = 6,
                 RoleId = 1,
                 MenuId = 6,
             },
             new RoleMenu {
                 Id = 7,
                 RoleId = 1,
                 MenuId = 7,
             },
             new RoleMenu {
                 Id = 8,
                 RoleId = 1,
                 MenuId = 8,
             },
             new RoleMenu {
                 Id = 9,
                 RoleId = 1,
                 MenuId = 9,
             },
             new RoleMenu {
                 Id = 10,
                 RoleId = 1,
                 MenuId = 10,
             },
             new RoleMenu {
                 Id = 11,
                 RoleId = 1,
                 MenuId = 11,
             },
             new RoleMenu {
                 Id = 12,
                 RoleId = 1,
                 MenuId = 12,
             },
             new RoleMenu {
                 Id = 13,
                 RoleId = 1,
                 MenuId = 13,
             },
             new RoleMenu {
                 Id = 14,
                 RoleId = 2,
                 MenuId = 1,
             }
            );
        }
    }
}
