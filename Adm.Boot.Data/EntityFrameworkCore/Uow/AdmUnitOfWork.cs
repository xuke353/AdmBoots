using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Adm.Boot.Data.EntityFrameworkCore.Uow {

    public class AdmUnitOfWork : IAdmUnitOfWork {
        private readonly AdmDbContext _dbContext;

        public AdmUnitOfWork(AdmDbContext dbContext) {
            _dbContext = dbContext;
        }

        public DbContext GetDbContext() {
            return _dbContext;
        }
        //开启事务
        public IDbContextTransaction Begin() {
            var scope = _dbContext.Database.BeginTransaction();
            return scope;
        }

        /// <summary>
        /// 提交事务保存
        /// </summary>
        /// <returns></returns>
        public int SaveChanges() {
            return _dbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync() {
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 回滚
        /// </summary>
        public void RollBackChanges() {
            var items = _dbContext.ChangeTracker.Entries().ToList();
            items.ForEach(o => o.State = EntityState.Unchanged);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        private bool _disposed = false;

        protected void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    _dbContext.Dispose();//随着工作单元的销毁而销毁
                }
            }
            _disposed = true;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Complete(IDbContextTransaction transaction) {
            if (transaction != null) {
                SaveChanges();
                transaction.Commit();
            }
        }

        public async Task CompleteAsync(IDbContextTransaction transaction) {
            if (transaction != null) {
                await SaveChangesAsync();
                transaction.Commit();
            }
        }
    }
}
