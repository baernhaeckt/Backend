﻿using Backend.Core.Hubs;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Web.Setup
{
    public static class SignalRSetup
    {
        public static void AddNewsfeed(this IServiceCollection services)
        {
            services.AddTransient<IEventStream, SignalREventStream>();
            services.AddSignalR();
        }
    }
}