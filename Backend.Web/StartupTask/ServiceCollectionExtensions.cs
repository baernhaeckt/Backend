﻿using Backend.Core.Startup;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Web.StartupTask
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStartupTask<T>(this IServiceCollection services)
            where T : class, IStartupTask
            => services.AddTransient<IStartupTask, T>();
    }
}
