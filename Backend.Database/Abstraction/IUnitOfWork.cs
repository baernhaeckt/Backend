﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Database.Abstraction
{
    public interface IUnitOfWork : IReader
    {
        Task DeleteAsync<TEntity>(Guid id) where TEntity : Entity, new();

        Task<TEntity> InsertAsync<TEntity>(TEntity record) where TEntity : Entity, new();

        Task InsertManyAsync<TEntity>(IEnumerable<TEntity> records) where TEntity : Entity, new();

        Task UpdateAsync<TEntity>(TEntity record) where TEntity : Entity, new();
    }
}