using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sciendo.Last.Fm;
using Sciendo.Topper.Contracts;
using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using Sciendo.Topper.Service;
using Sciendo.Topper.Service.Mappers;
using Sciendo.Topper.Store;
using Sciendo.Web;
using Serilog;
using System.Collections.Generic;

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
            services.AddSingleton(configuration.GetSection("fileStore").Get<FileStoreConfig>());
            services.AddSingleton(configuration.GetSection("pathToUrlConverter").Get<PathToUrlConverterConfig>());
            services.AddSingleton(configuration.GetSection("lastFm").Get<LastFmConfig>());
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IRepository<TopItem>, Repository<TopItem>>();
            services.AddScoped<IFileRepository<NamedPicture>, PictureRepository>();
            services.AddScoped<IWebGet<string>, WebStringGet>();
            services.AddScoped<IWebGet<byte[]>, WebBytesGet>();
            services.AddScoped<IEntryArtistImageProvider, EntryArtistImageProvider>();
            services.AddScoped<IMap<TopItem, Position>, MapTopItemToPosition>();
            services.AddScoped<IMap<TopItem, TopItemWithPictureUrl>, MapTopItemToTopItemWithPictureUrl>();
            services.AddScoped<IMap<IEnumerable<TopItem>, IEnumerable<TopItemWithPictureUrl>>, MapTopItemsToTopItemsWithPictureUrl>();
            services.AddScoped<IMap<TopItemWithPictureUrl, OverallEntry>, MapTopItemToOverallEntry>();
            services.AddScoped<IMap<IEnumerable<TopItemWithPictureUrl>, EntryTimeLine>, MapTopItemToEntryTimeLine>();
            services.AddScoped<IMap<IEnumerable<TopItemWithPictureUrl>, IEnumerable<EntryTimeLine>>, MapTopItemsToEntryTimeLines>();
            services.AddScoped<IMap<IEnumerable<TopItemWithPictureUrl>, IEnumerable<OverallEntry>>, MapTopItemsToOverallEntries>();
            services.AddScoped<IMapAggregateTwoEntries<TopItemWithPictureUrl,TopItem, OverallEntryEvolution>, MapTopItemToOverallEntryEvolution>();
            services.AddScoped<IMapAggregateTwoEntries<IEnumerable<TopItemWithPictureUrl>,IEnumerable<TopItem>, IEnumerable<OverallEntryEvolution>>, MapTopItemsToOverallEntriesEvolution>();
            services.AddScoped<IMapAggregateFourEntries<TopItemWithPictureUrl,TopItem, DayEntryEvolution>, MapTopItemToDayEntryEvolution>();
            services.AddScoped<IMapAggregateFourEntries<IEnumerable<TopItemWithPictureUrl>, IEnumerable<TopItem>, IEnumerable<DayEntryEvolution>>, MapTopItemsToDayEntriesEvolution>();
            services.AddScoped<IEntriesService, EntriesService>();
            services.AddScoped<IPictureReader, WebPictureReader>();
            services.AddScoped<IPictureReader, DataPictureReader>();
            services.AddScoped<IMap<ImageInfo, NamedPicture>, MapImageInfoToNamedPicture>();
            services.AddScoped<IImageService, ImageService>();
        }
    }
}
