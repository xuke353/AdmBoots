using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdmBoots.Domain.IRepositories {

    public interface ISqlExecuterRepository {

        public IQueryable<T> SqlQuery<T>(string sql) where T : class;

        public IQueryable<T> SqlQuery<T>(string sql, params object[] parameters) where T : class;

        public int Execute(string sql, params object[] parameters);

        public int Execute(string sql);
    }
}
