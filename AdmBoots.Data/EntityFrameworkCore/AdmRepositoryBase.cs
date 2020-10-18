using AdmBoots.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Infrastructure.Framework.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AdmBoots.Infrastructure.Extensions;
using AdmBoots.Infrastructure.CustomExceptions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AdmBoots.Data.EntityFrameworkCore {

    public class AdmRepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey> {
        public AdmDbContext Context { get; }
        public virtual DbSet<TEntity> Table => Context.Set<TEntity>();

        public AdmRepositoryBase(AdmDbContext context) {
            Context = context;
        }

        public IQueryable<TEntity> GetAll() {
            return GetAllIncluding();
        }

        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors) {
            IQueryable<TEntity> queryable = Table.AsQueryable();
            if (!propertySelectors.IsNullOrEmpty()) {
                foreach (Expression<Func<TEntity, object>> navigationPropertyPath in propertySelectors) {
                    queryable = queryable.Include(navigationPropertyPath);
                }
            }
            return queryable;
        }

        public List<TEntity> GetAllList() {
            return GetAll().ToList();
        }

        public async Task<List<TEntity>> GetAllListAsync() {
            return await GetAll().ToListAsync();
        }

        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate) {
            return GetAll().Where(predicate).ToList();
        }

        public async Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate) {
            return await GetAll().Where(predicate).ToListAsync();
        }

        public T Query<T>(Func<IQueryable<TEntity>, T> queryMethod) {
            return queryMethod(GetAll());
        }

        public TEntity Get(TPrimaryKey id) {
            return FirstOrDefault(id) ?? throw new EntityNotFoundException(typeof(TEntity), id);
        }

        public async Task<TEntity> GetAsync(TPrimaryKey id) {
            return (await FirstOrDefaultAsync(id)) ?? throw new EntityNotFoundException(typeof(TEntity), id);
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate) {
            return GetAll().Single(predicate);
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate) {
            return await GetAll().SingleAsync(predicate);
        }

        public TEntity FirstOrDefault(TPrimaryKey id) {
            return GetAll().FirstOrDefault(CreateEqualityExpressionForId(id));
        }

        public async Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id) {
            return await GetAll().FirstOrDefaultAsync(CreateEqualityExpressionForId(id));
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate) {
            return GetAll().FirstOrDefault(predicate);
        }

        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate) {
            return Task.FromResult(FirstOrDefault(predicate));
        }

        public TEntity Load(TPrimaryKey id) {
            return Get(id);
        }

        public TEntity Insert(TEntity entity) {
            return Table.Add(entity).Entity;
        }

        public Task<TEntity> InsertAsync(TEntity entity) {
            return Task.FromResult(Insert(entity));
        }

        public TPrimaryKey InsertAndGetId(TEntity entity) {
            entity = Insert(entity);
            if (entity.IsTransient()) {
                Context.SaveChanges();
            }
            return entity.Id;
        }

        public async Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity) {
            entity = await InsertOrUpdateAsync(entity);

            if (entity.IsTransient()) {
                await Context.SaveChangesAsync();
            }

            return entity.Id;
        }

        public TEntity InsertOrUpdate(TEntity entity) {
            if (!entity.IsTransient()) {
                return Update(entity);
            }
            return Insert(entity);
        }

        public async Task<TEntity> InsertOrUpdateAsync(TEntity entity) {
            return (!entity.IsTransient()) ? (await UpdateAsync(entity)) : (await InsertAsync(entity));
        }

        public TPrimaryKey InsertOrUpdateAndGetId(TEntity entity) {
            return InsertOrUpdate(entity).Id;
        }

        public async Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity) {
            entity = await InsertOrUpdateAsync(entity);

            if (entity.IsTransient()) {
                await Context.SaveChangesAsync();
            }

            return entity.Id;
        }

        public TEntity Update(TEntity entity) {
            AttachIfNot(entity);
            Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public Task<TEntity> UpdateAsync(TEntity entity) {
            entity = Update(entity);
            return Task.FromResult(entity);
        }

        public TEntity Update(TPrimaryKey id, Action<TEntity> updateAction) {
            TEntity val = Get(id);
            updateAction(val);
            return val;
        }

        public async Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task> updateAction) {
            TEntity entity = await GetAsync(id);
            await updateAction(entity);
            return entity;
        }

        public void Delete(TEntity entity) {
            AttachIfNot(entity);
            Table.Remove(entity);
        }

        public Task DeleteAsync(TEntity entity) {
            Delete(entity);
            return Task.FromResult(0);
        }

        public void Delete(TPrimaryKey id) {
            TEntity fromChangeTrackerOrNull = GetFromChangeTrackerOrNull(id);
            if (fromChangeTrackerOrNull != null) {
                Delete(fromChangeTrackerOrNull);
                return;
            }
            fromChangeTrackerOrNull = FirstOrDefault(id);
            if (fromChangeTrackerOrNull != null) {
                Delete(fromChangeTrackerOrNull);
            }
        }

        public Task DeleteAsync(TPrimaryKey id) {
            Delete(id);
            return Task.CompletedTask;
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate) {
            foreach (var entity in GetAllList(predicate)) {
                Delete(entity);
            }
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate) {
            var entities = await GetAllListAsync(predicate);

            foreach (var entity in entities) {
                await DeleteAsync(entity);
            }
        }

        public int Count() {
            return GetAll().Count();
        }

        public async Task<int> CountAsync() {
            return await GetAll().CountAsync();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate) {
            return GetAll().Count(predicate);
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate) {
            return Task.FromResult(Count(predicate));
        }

        public Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id) {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity));
            return Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(Expression.PropertyOrField(parameterExpression, "Id"), Expression.Constant(id, typeof(TPrimaryKey))), new ParameterExpression[1]
            {
                parameterExpression
            });
        }

        protected void AttachIfNot(TEntity entity) {
            var entry = Context.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null) {
                return;
            }

            Table.Attach(entity);
        }

        private TEntity GetFromChangeTrackerOrNull(TPrimaryKey id) {
            return Context.ChangeTracker.Entries().FirstOrDefault(delegate (EntityEntry ent) {
                if (ent.Entity is TEntity) {
                    return EqualityComparer<TPrimaryKey>.Default.Equals(id, (ent.Entity as TEntity).Id);
                }
                return false;
            })?.Entity as TEntity;
        }
    }

    /// <summary>
    /// 默认主键为int类型的仓储
    /// 实现方法与AdmRepositoryBase<TEntity, TPrimaryKey>相同
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class AdmRepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class, IEntity<int> {
        public AdmDbContext Context { get; }
        public virtual DbSet<TEntity> Table => Context.Set<TEntity>();

        public AdmRepositoryBase(AdmDbContext context) {
            Context = context;
        }

        public IQueryable<TEntity> GetAll() {
            return GetAllIncluding();
        }

        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors) {
            IQueryable<TEntity> queryable = Table.AsQueryable();
            if (!propertySelectors.IsNullOrEmpty()) {
                foreach (Expression<Func<TEntity, object>> navigationPropertyPath in propertySelectors) {
                    queryable = queryable.Include(navigationPropertyPath);
                }
            }
            return queryable;
        }

        public List<TEntity> GetAllList() {
            return GetAll().ToList();
        }

        public async Task<List<TEntity>> GetAllListAsync() {
            return await GetAll().ToListAsync();
        }

        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate) {
            return GetAll().Where(predicate).ToList();
        }

        public async Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate) {
            return await GetAll().Where(predicate).ToListAsync();
        }

        public T Query<T>(Func<IQueryable<TEntity>, T> queryMethod) {
            return queryMethod(GetAll());
        }

        public TEntity Get(int id) {
            return FirstOrDefault(id) ?? throw new EntityNotFoundException(typeof(TEntity), id);
        }

        public async Task<TEntity> GetAsync(int id) {
            return (await FirstOrDefaultAsync(id)) ?? throw new EntityNotFoundException(typeof(TEntity), id);
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate) {
            return GetAll().Single(predicate);
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate) {
            return await GetAll().SingleAsync(predicate);
        }

        public TEntity FirstOrDefault(int id) {
            return GetAll().FirstOrDefault(CreateEqualityExpressionForId(id));
        }

        public async Task<TEntity> FirstOrDefaultAsync(int id) {
            return await GetAll().FirstOrDefaultAsync(CreateEqualityExpressionForId(id));
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate) {
            return GetAll().FirstOrDefault(predicate);
        }

        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate) {
            return Task.FromResult(FirstOrDefault(predicate));
        }

        public TEntity Load(int id) {
            return Get(id);
        }

        public TEntity Insert(TEntity entity) {
            return Table.Add(entity).Entity;
        }

        public Task<TEntity> InsertAsync(TEntity entity) {
            return Task.FromResult(Insert(entity));
        }

        public int InsertAndGetId(TEntity entity) {
            entity = Insert(entity);
            if (entity.IsTransient()) {
                Context.SaveChanges();
            }
            return entity.Id;
        }

        public async Task<int> InsertAndGetIdAsync(TEntity entity) {
            entity = await InsertOrUpdateAsync(entity);

            if (entity.IsTransient()) {
                await Context.SaveChangesAsync();
            }

            return entity.Id;
        }

        public TEntity InsertOrUpdate(TEntity entity) {
            if (!entity.IsTransient()) {
                return Update(entity);
            }
            return Insert(entity);
        }

        public async Task<TEntity> InsertOrUpdateAsync(TEntity entity) {
            return (!entity.IsTransient()) ? (await UpdateAsync(entity)) : (await InsertAsync(entity));
        }

        public int InsertOrUpdateAndGetId(TEntity entity) {
            return InsertOrUpdate(entity).Id;
        }

        public async Task<int> InsertOrUpdateAndGetIdAsync(TEntity entity) {
            entity = await InsertOrUpdateAsync(entity);

            if (entity.IsTransient()) {
                await Context.SaveChangesAsync();
            }

            return entity.Id;
        }

        public TEntity Update(TEntity entity) {
            AttachIfNot(entity);
            Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public Task<TEntity> UpdateAsync(TEntity entity) {
            entity = Update(entity);
            return Task.FromResult(entity);
        }

        public TEntity Update(int id, Action<TEntity> updateAction) {
            TEntity val = Get(id);
            updateAction(val);
            return val;
        }

        public async Task<TEntity> UpdateAsync(int id, Func<TEntity, Task> updateAction) {
            TEntity entity = await GetAsync(id);
            await updateAction(entity);
            return entity;
        }

        public void Delete(TEntity entity) {
            AttachIfNot(entity);
            Table.Remove(entity);
        }

        public Task DeleteAsync(TEntity entity) {
            Delete(entity);
            return Task.FromResult(0);
        }

        public void Delete(int id) {
            TEntity fromChangeTrackerOrNull = GetFromChangeTrackerOrNull(id);
            if (fromChangeTrackerOrNull != null) {
                Delete(fromChangeTrackerOrNull);
                return;
            }
            fromChangeTrackerOrNull = FirstOrDefault(id);
            if (fromChangeTrackerOrNull != null) {
                Delete(fromChangeTrackerOrNull);
            }
        }

        public Task DeleteAsync(int id) {
            Delete(id);
            return Task.CompletedTask;
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate) {
            foreach (var entity in GetAllList(predicate)) {
                Delete(entity);
            }
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate) {
            var entities = await GetAllListAsync(predicate);

            foreach (var entity in entities) {
                await DeleteAsync(entity);
            }
        }

        public int Count() {
            return GetAll().Count();
        }

        public async Task<int> CountAsync() {
            return await GetAll().CountAsync();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate) {
            return GetAll().Count(predicate);
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate) {
            return Task.FromResult(Count(predicate));
        }

        public Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(int id) {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity));
            return Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(Expression.PropertyOrField(parameterExpression, "Id"), Expression.Constant(id, typeof(int))), new ParameterExpression[1]
            {
                parameterExpression
            });
        }

        protected void AttachIfNot(TEntity entity) {
            var entry = Context.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null) {
                return;
            }

            Table.Attach(entity);
        }

        private TEntity GetFromChangeTrackerOrNull(int id) {
            return Context.ChangeTracker.Entries().FirstOrDefault(delegate (EntityEntry ent) {
                if (ent.Entity is TEntity) {
                    return EqualityComparer<int>.Default.Equals(id, (ent.Entity as TEntity).Id);
                }
                return false;
            })?.Entity as TEntity;
        }
    }
}
