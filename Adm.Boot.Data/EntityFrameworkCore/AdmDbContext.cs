using Adm.Boot.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Adm.Boot.Data.EntityFrameworkCore {
    public class AdmDbContext : DbContext {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<RoleMenu> RoleMenus { get; set; }
        public AdmDbContext(DbContextOptions<AdmDbContext> options)
         : base(options) {
        }
    }
}
