﻿using System.Diagnostics.CodeAnalysis;
using Backend.Core;
using Backend.Core.Features.Awards;
using Backend.Core.Features.Baseline;
using Backend.Core.Features.Friendship;
using Backend.Core.Features.Newsfeed;
using Backend.Core.Features.Newsfeed.Hubs;
using Backend.Core.Features.Partner;
using Backend.Core.Features.Points;
using Backend.Core.Features.Quiz;
using Backend.Core.Features.UserManagement;
using Backend.Infrastructure.Email;
using Backend.Infrastructure.Geolocation;
using Backend.Infrastructure.Hosting;
using Backend.Infrastructure.Persistence;
using Backend.Infrastructure.Security;
using Backend.Web.Diagnostics;
using Backend.Web.Middleware;
using Backend.Web.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Silverback.Messaging.Configuration;

namespace Backend.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        private readonly IHostEnvironment _hostEnvironment;

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();

            services.AddApiDocumentation();
            services.AddSilverback().UseModel();
            services.AddMvcWithCors();
            services.AddJwtAuthentication();

            IHealthChecksBuilder healthChecksBuilder = services.AddHealthChecks();

            // Infrastructure
            services.AddInfrastructurePersistence(_configuration, healthChecksBuilder);
            services.AddInfrastructureEmail(_configuration, _hostEnvironment);
            services.AddInfrastructureHosting();
            services.AddInfrastructureGeolocation(_configuration, _hostEnvironment);
            services.AddInfrastructureSecurity(_hostEnvironment);

            // Features
            services.AddFeatureUserManagement(_hostEnvironment);
            services.AddFeatureBaseline();
            services.AddFeatureFriendship();
            services.AddFeatureNewsfeed();
            services.AddFeaturePartner(_hostEnvironment);
            services.AddFeaturePoints();
            services.AddFeatureQuiz();
            services.AddFeatureAward();

            services.AddHostedService<StartupTaskRunner>();
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Framework demand.")]
        public void Configure(IApplicationBuilder app, IHostEnvironment env, BusConfigurator busConfigurator)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(Localization.DefaultCultureInfo.TwoLetterISOLanguageName),
                SupportedCultures = Localization.SupportedCultures, // Formatting numbers, dates, etc.
                SupportedUICultures = Localization.SupportedCultures // UI strings that are localized.
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Metadata.ApplicationName} API V1"); });

            app.UseCors(x =>
                x.AllowAnyMethod()
                    .WithOrigins("http://localhost:8080", "https://baernhaeckt.z16.web.core.windows.net")
                    .AllowAnyHeader()
                    .AllowCredentials());

            app.UseRouting();

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = HealthCheckJsonResponseWriter.WriteHealthCheckJsonResponse
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
                endpoints.MapHub<NewsfeedHub>("/newsfeed");
            });

            busConfigurator.ScanSubscribers();
        }
    }
}