using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sciendo.Topper.Contracts;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Service;
using Sciendo.Topper.Store;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sciendo.Topper.Api.Extensions
{
    public static class ServicesExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }

        public static void ConfigureLogger(this IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddSerilog(new LoggerConfiguration()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger()));

        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
               
            });
        }

        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IAdminService, AdminService>();
            //services.AddScoped<IRepository<TopItem>, Repository<TopItem>>(s=>new Repository<TopItem>(cosmosDbConfig));

        }
    }
}
