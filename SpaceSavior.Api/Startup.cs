using System;
using System.IO;
using App.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using SpaceSavior.Business;
using SpaceSavior.Business.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace SpaceSavior.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc(options => options.AddMetricsResourceFilter());
            services
                .AddMetrics()
                .AddJsonSerialization()
                .AddJsonEnvironmentInfoSerialization() // https://github.com/alhardy/AppMetrics/issues/180
                .AddHealthChecks(healthCheckFactory =>
                {
                    healthCheckFactory.RegisterPingHealthCheck("Can we ping google.com within one second?", "google.com", TimeSpan.FromSeconds(1));
                })
                .AddMetricsMiddleware(Configuration.GetSection("AspNetMetrics"))
                .AddRequiredAspNetPlatformServices();

            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info
                    {
                        Title = "SpaceSavior API",
                        Version = "v1"
                    });
                    string filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "SpaceSavior.Api.xml");
                    if (File.Exists(filePath))
                    {
                        c.IncludeXmlComments(filePath);
                    }
                }
            );
            services.AddSpaceSaviorServices(Configuration.GetSection("RateConfigurationSettings").Get<RateConfigurationSettings>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMetrics();

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
            });
        }
    }
}
