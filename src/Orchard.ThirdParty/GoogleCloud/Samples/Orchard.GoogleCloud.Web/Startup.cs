﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Orchard.GoogleCloud.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("logging.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"logging.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddOrchardCms()
                .ConfigureModules(cf => {
                    // Provided at startup, and disabled after.
                    // TODO: Provide configuration... Settings file?
                    cf.WithDefaultFeatures(
                        "Orchard.GoogleCloud.Diagnostics.ErrorReporting",
                        "Orchard.GoogleCloud.Diagnostics.Logging",
                        "Orchard.GoogleCloud.Diagnostics.Trace");
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            loggerFactory.AddConsole(Configuration);

            if (env.IsDevelopment())
            {
                loggerFactory.AddDebug();
            }

            app.UseModules();
        }
    }
}
