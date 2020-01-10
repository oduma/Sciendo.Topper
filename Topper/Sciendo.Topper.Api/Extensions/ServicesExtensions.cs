using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sciendo.MusicStory;
using Sciendo.OAuth1_0;
using Sciendo.Topper.Contracts;
using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using Sciendo.Topper.Service;
using Sciendo.Topper.Service.Mappers;
using Sciendo.Topper.Source;
using Sciendo.Topper.Source.DataTypes.MusicStory;
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
            services.AddSingleton(configuration.GetSection("fileStore").Get<FileStoreConfig>());
            services.AddSingleton(configuration.GetSection("ms").Get<MSConfig>());
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IRepository<TopItem>, Repository<TopItem>>();
            services.AddScoped<IFileRepository<NamedPicture>, PictureRepository>();
            services.AddScoped<IContentProvider<ArtistsSummary>, ContentProvider<ArtistsSummary>>();
            services.AddScoped<IArtistsSummaryProvider, ArtistsSummaryProvider>();
            services.AddScoped<IMusicStoryProvider<string>, MusicStoryStringProvider>();
            services.AddScoped<IOAuthUrlGenerator, OAuthUrlGenerator>();
            services.AddScoped<IUrlProvider, UrlProvider>();
            services.AddScoped<IContentProvider<PictureUrlSummary>, ContentProvider<PictureUrlSummary>>();
            services.AddScoped<IPictureUrlsSummaryProvider, PictureUrlsSummaryProvider>();
            services.AddScoped<IMusicStoryProvider<byte[]>, MusicStoryByteArrayProvider>();
            services.AddScoped<IArtistImageProvider, ArtistImageProvider>();
            services.AddScoped<IEntryArtistImageProvider, EntryArtistImageProvider>();
            services.AddScoped<IMap<TopItem, Position>, MapTopItemToPosition>();
            services.AddScoped<IMap<TopItem, EntryBase>, MapTopItemToBaseEntry>();
            services.AddScoped<IMap<TopItem, OverallEntry>, MapTopItemToOverallEntry>();
            services.AddScoped<IMap<IEnumerable<TopItem>, EntryTimeLine>, MapTopItemToEntryTimeLine>();
            services.AddScoped<IMap<IEnumerable<TopItem>, IEnumerable<EntryTimeLine>>, MapTopItemsToEntryTimeLines>();
            services.AddScoped<IMap<IEnumerable<TopItem>, IEnumerable<OverallEntry>>, MapTopItemsToOverallEntries>();
            services.AddScoped<IMapAggregateTwoEntries<TopItem, OverallEntryEvolution>, MapTopItemToOverallEntryEvolution>();
            services.AddScoped<IMapAggregateTwoEntries<IEnumerable<TopItem>, IEnumerable<OverallEntryEvolution>>, MapTopItemsToOverallEntriesEvolution>();
            services.AddScoped<IMapAggregateFourEntries<TopItem, DayEntryEvolution>, MapTopItemToDayEntryEvolution>();
            services.AddScoped<IMapAggregateFourEntries<IEnumerable<TopItem>, IEnumerable<DayEntryEvolution>>, MapTopItemsToDayEntriesEvolution>();
            services.AddScoped<IEntriesService, EntriesService>();
        }
    }
}
