using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdmBoots.Data.EntityFrameworkCore.Uow;
using AdmBoots.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace AdmBoots.Data.EntityFrameworkCore.Repositories {

    public class SqlExecuterRepository : ISqlExecuterRepository {
        private readonly IAdmUnitOfWork _dbContextProvider;

        public SqlExecuterRepository(IAdmUnitOfWork dbContextProvider) {
            _dbContextProvider = dbContextProvider;
        }

        public int Execute(string sql, params object[] parameters) {
            return _dbContextProvider.GetDbContext().Database.ExecuteSqlRaw(sql, parameters);
        }

        public int Execute(string sql) {
            return _dbContextProvider.GetDbContext().Database.ExecuteSqlRaw(sql);
        }

        public IQueryable<T> SqlQuery<T>(string sql) where T : class {
            return _dbContextProvider.GetDbContext().Set<T>().FromSqlRaw(sql);
        }

        public IQueryable<T> SqlQuery<T>(string sql, params object[] parameters) where T : class {
            return _dbContextProvider.GetDbContext().Set<T>().FromSqlRaw(sql, parameters);
        }
    }
}
