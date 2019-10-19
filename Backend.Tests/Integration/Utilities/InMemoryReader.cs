﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Backend.Core.Entities;
using Backend.Infrastructure.Persistence.Abstraction;

namespace Backend.Tests.Integration.Utilities
{
    public class InMemoryReader : IReader
    {
        private readonly IDictionary<Type, IList<object>> _entities;

        public InMemoryReader()
        {
            _entities = new ConcurrentDictionary<Type, IList<object>>();
            IEnumerable<Type> allEntityTypes = typeof(User).Assembly.GetTypes().Where(t => typeof(Entity).IsAssignableFrom(t));
            foreach (Type allEntityType in allEntityTypes)
            {
                _entities.Add(allEntityType, new List<object>());
            }
        }

        protected IDictionary<Type, IList<object>> Entities => _entities;

        public Task<long> CountAsync<TEntity>()
            where TEntity : Entity, new() =>
            Task.FromResult(_entities[typeof(TEntity)].LongCount());

        public Task<long> CountAsync<TEntity>(Expression<Func<TEntity, bool>> filterPredicate)
            where TEntity : Entity, new() =>
            Task.FromResult(_entities[typeof(TEntity)].LongCount(record => filterPredicate.Compile().Invoke((TEntity)record)));

        public Task<IEnumerable<TEntity>> GetAllAsync<TEntity>()
            where TEntity : Entity, new() =>
            Task.FromResult(_entities[typeof(TEntity)].Cast<TEntity>());

        public Task<TEntity> GetByIdOrDefaultAsync<TEntity>(Guid id)
            where TEntity : Entity, new() =>
            Task.FromResult(_entities[typeof(TEntity)].Cast<TEntity>().SingleOrDefault(e => e.Id == id));

        public Task<TEntity> GetByIdOrThrowAsync<TEntity>(Guid id)
            where TEntity : Entity, new() =>
            Task.FromResult(_entities[typeof(TEntity)].Cast<TEntity>().Single(e => e.Id == id));

        public Task<TProjection?> GetByIdOrDefaultAsync<TEntity, TProjection>(Guid id, Expression<Func<TEntity, TProjection>> selectPredicate)
            where TEntity : Entity, new()
            where TProjection : class =>
            SingleOrDefaultAsync(e => e.Id == id, selectPredicate);

        public Task<TProjection> GetByIdOrThrowAsync<TEntity, TProjection>(Guid id, Expression<Func<TEntity, TProjection>> selectPredicate)
            where TEntity : Entity, new()
            where TProjection : class =>
            Task.FromResult(selectPredicate.Compile().Invoke(_entities[typeof(TEntity)].Cast<TEntity>().Single(e => e.Id == id)));

        public Task<IEnumerable<TEntity>> WhereAsync<TEntity>(Expression<Func<TEntity, bool>> filterPredicate)
            where TEntity : Entity, new() =>
            Task.FromResult(_entities[typeof(TEntity)].Cast<TEntity>().Where(filterPredicate.Compile()));

        public Task<TEntity> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> filterPredicate)
            where TEntity : Entity, new() =>
            Task.FromResult(_entities[typeof(TEntity)].Cast<TEntity>().FirstOrDefault(filterPredicate.Compile()));

        public Task<TEntity> SingleOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> filterPredicate)
            where TEntity : Entity, new() =>
            Task.FromResult(_entities[typeof(TEntity)].Cast<TEntity>().SingleOrDefault(filterPredicate.Compile()));

        public Task<TProjection?> SingleOrDefaultAsync<TEntity, TProjection>(Expression<Func<TEntity, bool>> filterPredicate, Expression<Func<TEntity, TProjection>> selectPredicate)
            where TEntity : Entity, new()
            where TProjection : class
        {
            TEntity queryResult = _entities[typeof(TEntity)].Cast<TEntity>().SingleOrDefault(filterPredicate.Compile());
            if (queryResult == null)
            {
                return Task.FromResult((TProjection?)null);
            }

            TProjection? projection = selectPredicate.Compile().Invoke(queryResult);
            return Task.FromResult((TProjection?)projection);
        }

        public Task<TEntity> SingleAsync<TEntity>(Expression<Func<TEntity, bool>> filterPredicate)
            where TEntity : Entity, new() =>
            Task.FromResult(_entities[typeof(TEntity)].Cast<TEntity>().Single(filterPredicate.Compile()));

        public Task<TProjection> SingleAsync<TEntity, TProjection>(Expression<Func<TEntity, bool>> filterPredicate, Expression<Func<TEntity, TProjection>> selectPredicate)
            where TEntity : Entity, new()
            where TProjection : class =>
            Task.FromResult(selectPredicate.Compile().Invoke(_entities[typeof(TEntity)].Cast<TEntity>().Single(filterPredicate.Compile())));
    }
}
