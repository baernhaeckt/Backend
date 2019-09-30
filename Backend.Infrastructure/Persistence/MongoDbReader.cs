﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Backend.Infrastructure.Persistence.Abstraction;
using MongoDB.Driver;

namespace Backend.Infrastructure.Persistence
{
    internal class MongoDbReader : IReader
    {
        public MongoDbReader(DbContextFactory dbContextFactory)
        {
            DbContextFactory = dbContextFactory;
        }

        protected DbContextFactory DbContextFactory { get; }

        public virtual async Task<TEntity> GetByIdOrDefaultAsync<TEntity>(Guid id)
            where TEntity : Entity, new()
        {
            DbContext dbContext = DbContextFactory.Create();
            FilterDefinition<TEntity> filter = new ExpressionFilterDefinition<TEntity>(u => u.Id == id);
            TEntity entity = await dbContext.GetCollection<TEntity>().Find(filter).SingleOrDefaultAsync();
            return entity;
        }

        public async Task<TEntity> GetByIdOrThrowAsync<TEntity>(Guid id)
            where TEntity : Entity, new()
        {
            TEntity result = await GetByIdOrDefaultAsync<TEntity>(id);
            return result ?? throw new EntityNotFoundException(typeof(TEntity));
        }

        public async Task<long> CountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : Entity, new()
        {
            DbContext dbContext = DbContextFactory.Create();
            FilterDefinition<TEntity> filter = new ExpressionFilterDefinition<TEntity>(predicate);
            return await dbContext.GetCollection<TEntity>().CountDocumentsAsync(filter);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>()
            where TEntity : Entity, new()
        {
            DbContext dbContext = DbContextFactory.Create();
            return (await dbContext.GetCollection<TEntity>().FindAsync(FilterDefinition<TEntity>.Empty)).ToEnumerable();
        }

        public async Task<long> CountAsync<TEntity>()
            where TEntity : Entity, new()
        {
            DbContext dbContext = DbContextFactory.Create();
            return await dbContext.GetCollection<TEntity>().CountDocumentsAsync(FilterDefinition<TEntity>.Empty);
        }

        public async Task<IEnumerable<TEntity>> WhereAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : Entity, new()
        {
            DbContext dbContext = DbContextFactory.Create();
            FilterDefinition<TEntity> filter = new ExpressionFilterDefinition<TEntity>(predicate);
            return (await dbContext.GetCollection<TEntity>().FindAsync(filter)).ToEnumerable();
        }

        public async Task<TEntity> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : Entity, new()
        {
            DbContext dbContext = DbContextFactory.Create();
            FilterDefinition<TEntity> filter = new ExpressionFilterDefinition<TEntity>(predicate);
            return await (await dbContext.GetCollection<TEntity>().FindAsync(filter)).FirstOrDefaultAsync();
        }

        public async Task<TEntity> SingleOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : Entity, new()
        {
            DbContext dbContext = DbContextFactory.Create();
            FilterDefinition<TEntity> filter = new ExpressionFilterDefinition<TEntity>(predicate);
            return await (await dbContext.GetCollection<TEntity>().FindAsync(filter)).FirstOrDefaultAsync();
        }

        public async Task<TEntity> SingleAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : Entity, new()
        {
            TEntity result = await SingleOrDefaultAsync(predicate);
            return result ?? throw new EntityNotFoundException(typeof(TEntity));
        }
    }
}