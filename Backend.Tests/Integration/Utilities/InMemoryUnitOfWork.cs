﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Backend.Infrastructure.Persistence.Abstraction;

namespace Backend.Tests.Integration.Utilities
{
    public class InMemoryUnitOfWork : InMemoryReader, IUnitOfWork
    {
        public Task DeleteAsync<TEntity>(Guid id)
            where TEntity : Entity, new()
        {
            TEntity record = Entities[typeof(TEntity)].Cast<TEntity>().Single(e => e.Id == id);
            Entities[typeof(TEntity)].Remove(record);
            return Task.CompletedTask;
        }

        public Task<TEntity> InsertAsync<TEntity>(TEntity record)
            where TEntity : Entity, new()
        {
            if (record.Id == Guid.Empty)
            {
                record.Id = Guid.NewGuid();
            }

            Entities[typeof(TEntity)].Add(record);
            return Task.FromResult(record);
        }

        public Task InsertManyAsync<TEntity>(IEnumerable<TEntity> records)
            where TEntity : Entity, new()
        {
            foreach (TEntity record in records)
            {
                InsertAsync(record);
            }

            return Task.CompletedTask;
        }

        public async Task UpdateAsync<TEntity>(Guid id, object updateDefinition)
            where TEntity : Entity, new()
        {
            TEntity record = await GetByIdOrThrowAsync<TEntity>(id);
            foreach (PropertyInfo propertyInfo in updateDefinition.GetType().GetProperties())
            {
                if (propertyInfo.PropertyType.GetInterfaces()
                    .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ICollection<>)))
                {
                    object? propertyValue = record.GetType().GetProperty(propertyInfo.Name)?.GetValue(record);
                    if (propertyValue == null)
                    {
                        throw new InvalidOperationException("Property not found!");
                    }

                    MethodInfo? addMethod = propertyValue.GetType()?.GetMethod("Add");
                    if (addMethod == null)
                    {
                        // This is expected to be happen one day.
                        // As soon as this use cases exist, rethink the applied concept.
                        throw new InvalidOperationException("No Add method found!");
                    }

                    object values = propertyInfo.GetValue(updateDefinition) ?? Enumerable.Empty<object>();
                    foreach (object? value in (IEnumerable)values)
                    {
                        if (value == null)
                        {
                            continue;
                        }

                        addMethod.Invoke(propertyValue, new[] { value });
                    }
                }
                else
                {
                    record.GetType().GetProperty(propertyInfo.Name)?.SetValue(record, propertyInfo.GetValue(updateDefinition));
                }
            }
        }

        public async Task UpdateAsync<TEntity>(TEntity record)
            where TEntity : Entity, new()
        {
            await DeleteAsync<TEntity>(record.Id);
            await InsertAsync(record);
        }
    }
}