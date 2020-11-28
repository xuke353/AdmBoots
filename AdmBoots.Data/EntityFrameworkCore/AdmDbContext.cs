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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AdmBoots.Data.EntityFrameworkCore {

    public class AdmDbContext : DbContext {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<RoleMenu> RoleMenus { get; set; }
        public virtual DbSet<JobLog> JobLogs { get; set; }
        public virtual DbSet<MailSetting> MailSettings { get; set; }
        //演示
        //public virtual DbSet<Test> Tests { get; set; }

        public virtual DbSet<TableFieldInfo> Tests { get; set; }

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
            modelBuilder.GenerateSeedData();
            modelBuilder.Entity<TableFieldInfo>().HasNoKey();
            base.OnModelCreating(modelBuilder);
        }

        private void ApplyAdmConcepts() {
            var session = this.GetService<AdmSession>();
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

        protected virtual void CheckAndSetId(EntityEntry entry) {
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
