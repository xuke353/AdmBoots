using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AdmBoots.Data.EntityFrameworkCore.Uow {

    public interface IAdmUnitOfWork {
        DbContext GetDbContext();
        IDbContextTransaction Begin();

        int SaveChanges();

        Task<int> SaveChangesAsync();

        void Complete(IDbContextTransaction transaction);

        Task CompleteAsync(IDbContextTransaction transaction);

        void RollBackChanges();

        void Dispose();
    }
}
