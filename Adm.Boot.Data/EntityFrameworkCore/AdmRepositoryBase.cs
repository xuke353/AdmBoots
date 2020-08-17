using Adm.Boot.Domain.IRepositorys;
using System;
using System.Collections.Generic;
using System.Text;
using Adm.Boot.Infrastructure.Framework.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CollectionExtensions = Adm.Boot.Infrastructure.Extensions.CollectionExtensions;
using Adm.Boot.Infrastructure.Extensions;

namespace Adm.Boot.Data.EntityFrameworkCore
{
    public class AdmRepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {

        public AdmDbContext Context { get; }
        public virtual DbSet<TEntity> Table => Context.Set<TEntity>();
        public AdmRepositoryBase(AdmDbContext context)
        {
            Context = context;
        }

        public IQueryable<TEntity> GetAll()
        {
            return GetAllIncluding(Array.Empty<Expression<Func<TEntity, object>>>());
        }

        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            IQueryable<TEntity> queryable = Table.AsQueryable();
            //if (!CollectionExtensions.IsNullOrEmpty<Expression<Func<TEntity, object>>>((ICollection<Expression<Func<TEntity, object>>>)propertySelectors))
            if (!propertySelectors.IsNullOrEmpty())
            {
                foreach (Expression<Func<TEntity, object>> navigationPropertyPath in propertySelectors)
                {
                    queryable = queryable.Include(navigationPropertyPath);
                }
            }
            return queryable;
        }

        public List<TEntity> GetAllList()
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> GetAllListAsync()
        {
            throw new NotImplementedException();
        }

        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public T Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
        {
            throw new NotImplementedException();
        }

        public TEntity Get(TPrimaryKey id)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetAsync(TPrimaryKey id)
        {
            throw new NotImplementedException();
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public TEntity FirstOrDefault(TPrimaryKey id)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id)
        {
            throw new NotImplementedException();
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public TEntity Load(TPrimaryKey id)
        {
            throw new NotImplementedException();
        }

        public TEntity Insert(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> InsertAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public TPrimaryKey InsertAndGetId(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public TEntity InsertOrUpdate(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> InsertOrUpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public TPrimaryKey InsertOrUpdateAndGetId(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public TEntity Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public TEntity Update(TPrimaryKey id, Action<TEntity> updateAction)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task> updateAction)
        {
            throw new NotImplementedException();
        }

        public void Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(TPrimaryKey id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TPrimaryKey id)
        {
            throw new NotImplementedException();
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public long LongCount()
        {
            throw new NotImplementedException();
        }

        public Task<long> LongCountAsync()
        {
            throw new NotImplementedException();
        }

        public long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
