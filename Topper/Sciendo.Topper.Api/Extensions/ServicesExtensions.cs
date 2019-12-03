using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sciendo.Topper.Contracts;
using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Service;
using Sciendo.Topper.Service.Mappers;
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

        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            
            var logger = services.BuildServiceProvider().GetRequiredService<ILogger<Repository<TopItem>>>();
            services.AddSingleton(configuration.GetSection("cosmosDb").Get<CosmosDbConfig>());
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IRepository<TopItem>, Repository<TopItem>>();
            services.AddScoped<IMap<TopItem, Position>, MapTopItemToPosition>();
            services.AddScoped<IMap<TopItem, EntryBase>, MapTopItemToBaseEntry>();
            services.AddScoped<IMap<TopItem, OverallEntry>, MapTopItemToOverallEntry>();
            services.AddScoped<IMap<TopItem, DatedEntry>, MapTopItemToDatedEntry>();
            services.AddScoped<IMap<IEnumerable<TopItem>, IEnumerable<DatedEntry>>, MapTopItemsToDatedEntries>();
            services.AddScoped<IMap<IEnumerable<TopItem>, IEnumerable<OverallEntry>>, MapTopItemsToOverallEntries>();
            services.AddScoped<IMapAggregateTwoEntries<TopItem, OverallEntryEvolution>, MapTopItemToOverallEntryEvolution>();
            services.AddScoped<IMapAggregateTwoEntries<IEnumerable<TopItem>, IEnumerable<OverallEntryEvolution>>, MapTopItemsToOverallEntriesEvolution>();
            services.AddScoped<IMapAggregateFourEntries<TopItem, DayEntryEvolution>, MapTopItemToDayEntryEvolution>();
            services.AddScoped<IMapAggregateFourEntries<IEnumerable<TopItem>, IEnumerable<DayEntryEvolution>>, MapTopItemsToDayEntriesEvolution>();
            services.AddScoped<IEntriesService, EntriesService>();
        }
    }
}
