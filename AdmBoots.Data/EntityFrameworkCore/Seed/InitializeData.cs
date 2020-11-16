using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdmBoots.Domain;
using AdmBoots.Domain.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AdmBoots.Data.EntityFrameworkCore.Seed {
    /// <summary>
    /// 系统初始化时生成种子数据
    /// </summary>
    public static class InitializeData {
        /// <summary>
        /// PM> update-database -Context AdmDbContext
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void GenerateSeedData(this ModelBuilder modelBuilder) {
            //不在这里设置默认值，会报错
            modelBuilder.Entity<User>().Property(p => p.IsMaster).HasDefaultValue(false);
            modelBuilder.Entity<User>().HasData(SeedData.Users);
            modelBuilder.Entity<Role>().HasData(SeedData.Roles);
            modelBuilder.Entity<Menu>().HasData(SeedData.Menus);
            modelBuilder.Entity<UserRole>().HasData(SeedData.UserRoles);
            modelBuilder.Entity<RoleMenu>().HasData(SeedData.RoleMenus);
        }
        /// <summary>
        /// 初始化种子数据
        /// </summary>
        /// <param name="app"></param>
        public static void InitializeDatabase(this IApplicationBuilder app) {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope()) {
                var context = serviceScope.ServiceProvider.GetRequiredService<AdmDbContext>();
                context.Database.Migrate();
                if (!context.Users.Any()) {
                    foreach (var user in SeedData.Users) {
                        context.Users.Add(user);
                    }
                    context.SaveChanges();
                }

                if (!context.Roles.Any()) {
                    foreach (var role in SeedData.Roles) {
                        context.Roles.Add(role);
                    }
                    context.SaveChanges();
                }

                if (!context.Menus.Any()) {
                    foreach (var menu in SeedData.Menus) {
                        context.Menus.Add(menu);
                    }
                    context.SaveChanges();
                }

                if (!context.RoleMenus.Any()) {
                    foreach (var roleMenu in SeedData.RoleMenus) {
                        context.RoleMenus.Add(roleMenu);
                    }
                    context.SaveChanges();
                }

                if (!context.UserRoles.Any()) {
                    foreach (var userRole in SeedData.UserRoles) {
                        context.UserRoles.Add(userRole);
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
