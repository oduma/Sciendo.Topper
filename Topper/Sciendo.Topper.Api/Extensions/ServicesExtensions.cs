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
using System.Linq;

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
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<Repository<TopItem>>>();

            services.AddSingleton(configuration.GetSection("cosmosDb").Get<CosmosDbConfig>());
            services.AddSingleton(configuration.GetSection("fileStore").Get<FileStoreConfig>());
            services.AddSingleton(configuration.GetSection("pathToUrlConverter").Get<PathToUrlConverterConfig>());
            services.AddSingleton(configuration.GetSection("lastFm").Get<LastFmConfig>());
            services.AddSingleton<IRepository<TopItem>>(r => new Repository<TopItem>(r.GetRequiredService<ILogger<Repository<TopItem>>>(), r.GetRequiredService<CosmosDbConfig>(), r.GetRequiredService<CosmosDbConfig>().CosmosDbCollections.FirstOrDefault(c => c.TypeOfItem == "TopItem").CollectionId));
            services.AddScoped<IStoreManager, StoreManager>();
            services.AddSingleton<IRepository<TopItemWithPartitionKey>>(r => new Repository<TopItemWithPartitionKey>(r.GetRequiredService<ILogger<Repository<TopItemWithPartitionKey>>>(), r.GetRequiredService<CosmosDbConfig>(), r.GetRequiredService<CosmosDbConfig>().CosmosDbCollections.FirstOrDefault(c => c.TypeOfItem == "TopItemWithPartitionKey").CollectionId));
            services.AddScoped<IMapper<TopItem, TopItemWithPartitionKey>, MapTopItemToTopItemWithPartitionKey>();
            services.AddScoped<IOverallStoreManager, OverallStoreManager>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IFileRepository<NamedPicture>, PictureRepository>();
            services.AddScoped<IWebGet<string>, WebStringGet>();
            services.AddScoped<IWebGet<byte[]>, WebBytesGet>();
            services.AddScoped<IMap<DateTimeInterval, TimeInterval>, MapDateTimeIntervalToTimeInterval>();
            services.AddScoped<IMap<TopItem, Positions>, MapTopItemToPositions>();
            services.AddScoped<IEntryArtistImageProvider, EntryArtistImageProvider>();
            services.AddScoped<IMap<IEnumerable<TopItem>, EntryTimeLine>, MapTopItemToEntryTimeLine>();
            services.AddScoped<IMap<IEnumerable<TopItem>, IEnumerable<EntryTimeLine>>, MapTopItemsToEntryTimeLines>();
            services.AddScoped<IMap<TopItemWithPartitionKey, OverallEntry>, MapTopItemWithPartitionKeyToOverallEntry>();
            services.AddScoped<IMap<TopItemWithPartitionKey, Position>, MapTopItemWithPartitionKeyToPosition>();
            services.AddScoped<IMap<IEnumerable<TopItemWithPartitionKey>, IEnumerable<OverallEntry>>, MapTopItemsWithPartitionKeyToOverallEntries>();
            services.AddScoped<IMapAggregateTwoEntries<TopItemWithPartitionKey,TopItemWithPartitionKey, OverallEntryEvolution>, MapTopItemWithPartitionKeyToOverallEntryEvolution>();
            services.AddScoped<IMapAggregateTwoEntries<IEnumerable<TopItemWithPartitionKey>,IEnumerable<TopItemWithPartitionKey>, IEnumerable<OverallEntryEvolution>>, MapTopItemsWithPartitionKeyToOverallEntriesEvolution>();
            services.AddScoped<IMapAggregateTwoEntries<TopItem,TopItem, DayEntryEvolution>, MapTopItemToDayEntryEvolution>();
            services.AddScoped<IMapAggregateTwoEntries<IEnumerable<TopItem>, IEnumerable<TopItem>, IEnumerable<DayEntryEvolution>>, MapTopItemsToDayEntriesEvolution>();
            services.AddScoped<IEntriesService, EntriesService>();
            services.AddScoped<IPictureReader, WebPictureReader>();
            services.AddScoped<IPictureReader, DataPictureReader>();
            services.AddScoped<IMap<ImageInfo, NamedPicture>, MapImageInfoToNamedPicture>();
            services.AddScoped<IImageService, ImageService>();
        }
    }
}
