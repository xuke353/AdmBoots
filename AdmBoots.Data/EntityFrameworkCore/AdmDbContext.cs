using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdmBoots.Data.EntityFrameworkCore.Seed;
using AdmBoots.Domain.Models;
using AdmBoots.Infrastructure.CodeGenerator;
using AdmBoots.Infrastructure.CustomExceptions;
using AdmBoots.Infrastructure.Extensions;
using AdmBoots.Infrastructure.Framework.Interface;
using AdmBoots.Infrastructure.Framework.Web;
using AdmBoots.Infrastructure.Helper;
using AdmBoots.Infrastructure.Ioc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AdmBoots.Data.EntityFrameworkCore {

    public class AdmDbContext : DbContext {
        public DbSet<User> Users { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }
        public DbSet<JobLog> JobLogs { get; set; }
        public DbSet<MailSetting> MailSettings { get; set; }

        //演示
        public DbSet<Test> Tests { get; set; }

        public DbSet<TableFieldInfo> TableFieldInfos { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public AdmDbContext(DbContextOptions<AdmDbContext> options)
         : base(options) {
        }

        public override int SaveChanges() {
            try {
                ApplyAdmConcepts();
                return base.SaveChanges(); ;
            } catch (DbUpdateConcurrencyException ex) {
                throw new AdmDbConcurrencyException(ex.Message, ex);
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
            try {
                ApplyAdmConcepts();
                return base.SaveChangesAsync(cancellationToken);
            } catch (DbUpdateConcurrencyException ex) {
                throw new AdmDbConcurrencyException(ex.Message, ex);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            //初始化种子数据
            modelBuilder.GenerateSeedData();
            modelBuilder.Entity<TableFieldInfo>().HasNoKey();
            //modelBuilder.Ignore<TableFieldInfo>();
            modelBuilder.Ignore<Test>();
            base.OnModelCreating(modelBuilder);
        }

        private void ApplyAdmConcepts() {
            var session = IocManager.Current.Resolve<IAdmSession>();
            foreach (var entry in ChangeTracker.Entries().ToList()) {
                switch (entry.State) {
                    case EntityState.Added:
                        CheckAndSetId(entry);
                        AdmEntityAuditingHelper.SetCreationAuditProperties(entry.Entity, session);
                        break;

                    case EntityState.Modified:
                        CheckAndSetId(entry);
                        AdmEntityAuditingHelper.SetModificationAuditProperties(entry.Entity, session);
                        break;

                    case EntityState.Deleted:
                        CancelDeletionForSoftDelete(entry);
                        AdmEntityAuditingHelper.SetDeletionAuditProperties(entry.Entity, session);
                        break;
                }
            }
        }

        private static void CancelDeletionForSoftDelete(EntityEntry entry) {
            if (!(entry.Entity is ISoftDelete)) {
                return;
            }

            entry.Reload();
            entry.State = EntityState.Modified;
            entry.Entity.As<ISoftDelete>().IsDeleted = true;
        }

        protected void CheckAndSetId(EntityEntry entry) {
            //Set GUID Ids
            if (entry.Entity is IEntity<Guid> entity && entity.Id == Guid.Empty) {
                var idPropertyEntry = entry.Property("Id");

                if (idPropertyEntry != null && idPropertyEntry.Metadata.ValueGenerated == ValueGenerated.Never) {
                    entity.Id = CombGuid.NewGuid();
                }
            }
        }
    }
}
