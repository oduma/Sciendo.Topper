using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Sciendo.Config;
using Sciendo.Last.Fm;
using Sciendo.Topper.Source;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Notifier;
using Sciendo.Topper.Source.DataTypes;
using Sciendo.Topper.Store;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sciendo.Web;
using Sciendo.Topper.Domain.Entities;

namespace Sciendo.Topper
{
    class Program
    {
        static int Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            
            var serviceProvider = ConfigureLog(serviceCollection);
            var logger = serviceProvider.GetService<ILogger<Program>>();
            logger.LogInformation("Starting...");
            var topperConfig = ReadConfiguration(logger, args);
            serviceProvider = ConfigureServices(serviceCollection,topperConfig);


            var notifier = CreateNotifier(logger, topperConfig.EmailOptions, serviceProvider);

            if (!notifier.SendPreviousFailedEmails())
            {
                logger.LogInformation("Trying to send new email for today...");
                var todayTopItems = GetTodayTopItems(logger, serviceProvider, topperConfig.TopperLastFmConfig);

                List<TopItemWithPartitionKey> yearAggregate = new List<TopItemWithPartitionKey>();

                CalculateTopItemsAndGetAggregateItems(logger, serviceProvider, todayTopItems, yearAggregate);

                if (!yearAggregate.Any() && !todayTopItems.Any())
                {
                    logger.LogWarning("Nothing to send. No email sent.");
                    return -2;
                }
                if (notifier.ComposeAndSendMessage(todayTopItems, yearAggregate, topperConfig.DestinationEmail))
                {
                    logger.LogInformation("Check your email.");
                    return 0;
                }
                else
                {
                    logger.LogWarning("Email not sent.");
                    return -1;
                }
            }
            return 0;
        }

        public delegate ITopItemsProvider TopItemsProviderResolver(string key);
        public delegate RuleBase RulesResolver(string key);
        private static ServiceProvider ConfigureServices(ServiceCollection serviceCollection, TopperConfig topperConfig)
        {
            serviceCollection.AddSingleton<IRepository<TopItem>>(r => new Repository<TopItem>(r.GetRequiredService<ILogger<Repository<TopItem>>>(), topperConfig.CosmosDbConfig,topperConfig.CosmosDbConfig.CosmosDbCollections.FirstOrDefault(c=>c.TypeOfItem=="TopItem").CollectionId));
            serviceCollection.AddSingleton<IRepository<TopItemWithPartitionKey>>(r => new Repository<TopItemWithPartitionKey>(r.GetRequiredService<ILogger<Repository<TopItemWithPartitionKey>>>(), topperConfig.CosmosDbConfig, topperConfig.CosmosDbConfig.CosmosDbCollections.FirstOrDefault(c => c.TypeOfItem == "TopItemWithPartitionKey").CollectionId));
            serviceCollection.AddSingleton<IEmailSender>(e => new EmailSender(e.GetRequiredService <ILogger<EmailSender>>(), topperConfig.EmailOptions));
            serviceCollection.AddSingleton<INotificationManager>(n => new NotificationManager(n.GetRequiredService<ILogger<NotificationManager>>(), n.GetRequiredService<IEmailSender>(),"mail"));
            serviceCollection.AddSingleton<IUrlProvider>(u => new UrlProvider(u.GetRequiredService<ILogger<UrlProvider>>(), topperConfig.TopperLastFmConfig.ApiKey));
            serviceCollection.AddSingleton<IWebGet<string>, WebStringGet>();
            serviceCollection.AddTransient<IContentProvider<TopArtistsRootObject>, ContentProvider<TopArtistsRootObject>>();
            serviceCollection.AddTransient<LastFmTopArtistsProvider>();
            serviceCollection.AddTransient<IContentProvider<LovedTracksRootObject>, ContentProvider<LovedTracksRootObject>>();
            serviceCollection.AddTransient<LastFmLovedProvider>();
            serviceCollection.AddTransient<TopItemsProviderResolver>(serviceProvider=>key=> 
            {
                switch(key)
                {
                    case "LOVEDTRACK":
                        return serviceProvider.GetService<LastFmLovedProvider>();
                    case "TOPARTIST":
                        return serviceProvider.GetService<LastFmTopArtistsProvider>();
                    default:
                        throw new KeyNotFoundException();
                }
            });
            serviceCollection.AddTransient<ITopItemsAggregator, TopItemsAggregator>();
            serviceCollection.AddTransient<IStoreManager, StoreManager>();
            serviceCollection.AddTransient<IOverallStoreManager, OverallStoreManager>();

            serviceCollection.AddTransient<IRulesEngine, RulesEngine>();
            serviceCollection.AddTransient<ArtistScoreRule>(a=>new ArtistScoreRule(a.GetRequiredService<ILogger<ArtistScoreRule>>(),a.GetRequiredService<IRepository<TopItem>>(),topperConfig.TopperRulesConfig.RankingBonus));
            serviceCollection.AddTransient<LovedRule>(l=> new LovedRule(l.GetRequiredService<ILogger<LovedRule>>(), l.GetRequiredService<IRepository<TopItem>>(), topperConfig.TopperRulesConfig.LovedBonus));
            serviceCollection.AddTransient<RulesResolver>(serviceProvider=>key=>
            {
                switch(key)
                {
                    case "LOVEDTRACK":
                        return serviceProvider.GetService<LovedRule>();
                    case "TOPARTIST":
                        return serviceProvider.GetService<ArtistScoreRule>();
                    default:
                        throw new KeyNotFoundException();
                }
            });
            return serviceCollection.BuildServiceProvider();
        }

        private static void CalculateTopItemsAndGetAggregateItems(
            ILogger<Program> logger, 
            ServiceProvider serviceProvider, 
            List<TopItem> todayTopItems,
            List<TopItemWithPartitionKey> yearAggregate)
        {
            var dailyStoreManager = serviceProvider.GetService<IStoreManager>();
            var overallStoreManager = serviceProvider.GetService<IOverallStoreManager>();

            if (todayTopItems.Count > 0)
                CalculateAndStoreTodayItems(logger, serviceProvider, todayTopItems, dailyStoreManager, overallStoreManager);
            else
                logger.LogWarning("No top items for today no calculation of scores needed.");

            yearAggregate = overallStoreManager.GetAggregateHistoryOfScores(DateTime.Now.ToString("yyyy-MM-dd")).ToList();
            if (!yearAggregate.Any())
            {
                logger.LogWarning("No year aggregate retrieve.");
            }
            else
                logger.LogInformation("Items scored this year {0}", yearAggregate.Count);
        }

        private static TopperConfig ReadConfiguration(ILogger<Program> logger, string[] args)
        {
            var config =
                new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"{AppDomain.CurrentDomain.FriendlyName}.json")
                    .AddCommandLine(args)
                    .Build();
            TopperConfig topperConfig;
            try
            {
                topperConfig = new ConfigurationManager<TopperConfig>().GetConfiguration(config);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Something happened here!");
                throw;
            }

            return topperConfig;
        }

        private static ServiceProvider ConfigureLog(IServiceCollection services)
        {
            return services.AddLogging(configure => configure.AddSerilog(new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger())).BuildServiceProvider();
        }

        private static void CalculateAndStoreTodayItems(
            ILogger<Program> logger, 
            ServiceProvider serviceProvider, 
            List<TopItem> todayTopItems,
            IStoreManager dailyStoreManager,
            IOverallStoreManager overallStoreManager)
        {
            logger.LogInformation("Calculating scores for today's items...");
            CalculateDailyTopItems(logger, serviceProvider, todayTopItems);
            var todaysProcessedItems = overallStoreManager.AdvanceOverallItems(todayTopItems.FirstOrDefault().Date, todayTopItems.ToArray(), out int totalRecordsAffectedForTheDay);
            logger.LogInformation("Items processed: {0}", totalRecordsAffectedForTheDay);
            try
            {
                dailyStoreManager.UpdateItems(todaysProcessedItems);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Cannot persist items in store.");
                throw;
            }
        }

        private static void CalculateDailyTopItems(ILogger<Program> logger, ServiceProvider serviceProvider, List<TopItem> todayTopItems)
        {
            var rulesEngine = serviceProvider.GetService<IRulesEngine>();
            rulesEngine.AddRule(serviceProvider.GetService<RulesResolver>()("TOPARTIST"));
            rulesEngine.AddRule(serviceProvider.GetService<RulesResolver>()("LOVEDTRACK"));
            foreach (var todayTopItem in todayTopItems)
            {
                logger.LogInformation("Calculating Score for {0}", todayTopItem.Name);
                rulesEngine.ApplyAllRules(todayTopItem);
                todayTopItem.Year = todayTopItem.Date.Year.ToString();
            }
        }

        private static List<TopItem> GetTodayTopItems(ILogger<Program> logger,  ServiceProvider serviceProvider,
            LastFmConfig topperLastFmConfig)
        {
            logger.LogInformation("Getting new top items from last.fm ...");

            TopItemsProviderResolver serviceResolver = serviceProvider.GetService<TopItemsProviderResolver>();

            List<TopItem> todayTopItems;
            IUrlProvider urlProvider;
            try
            {
                urlProvider = serviceProvider.GetService<IUrlProvider>();
            }
            catch (Exception e)
            {
                logger.LogError(e,"Cannot use urlProvider...");
                throw;
            }

            ITopItemsProvider lastFmTopArtistsProvider;
            try
            {
                lastFmTopArtistsProvider = serviceResolver("TOPARTIST");
            }
            catch (Exception e)
            {
                logger.LogError(e,"Cannot use LastFmTopArtistProvider");
                throw;
            }

            ITopItemsProvider lastFmLovedProvider;
            try
            {
                lastFmLovedProvider = serviceResolver("LOVEDTRACK");
            }
            catch (Exception e)
            {
                logger.LogError(e,"Cannot use LastFmLovesProvider");
                throw;
            }

            var topItemsAggregator = serviceProvider.GetService<ITopItemsAggregator>();
            topItemsAggregator.RegisterProvider(lastFmTopArtistsProvider);
            topItemsAggregator.RegisterProvider(lastFmLovedProvider);

            try
            {
                todayTopItems = topItemsAggregator.GetItems(topperLastFmConfig.UserName);
            }
            catch (Exception e)
            {
                logger.LogError(e,"Cannot Retrieve items from at least one source.");
                throw;
            }
            if(!todayTopItems.Any())
                logger.LogWarning("Todays top items not retrieved.");
            else
                logger.LogInformation("Retrieved {0} items.", todayTopItems.Count);
            return todayTopItems;
        }

        private static INotificationManager CreateNotifier(Microsoft.Extensions.Logging.ILogger<Program> logger, 
            EmailConfig emailConfig,
            ServiceProvider serviceProvider)
        {
            logger.LogInformation("Creating notifier for {0} ...",emailConfig.Domain);
            INotificationManager notifier;
            try
            {
                notifier = serviceProvider.GetService<INotificationManager>();
            }
            catch (Exception e)
            {
                logger.LogError(e,"Cannot create notifier.");
                throw;
            }
            logger.LogInformation("Notifier created.");
            return notifier;
        }

        private static void StoreLogic_Progress(object sender, ProgressEventArgs e)
        {
            Console.WriteLine("{0} item {1}; {2}; {3}.", e.Status, e.TopItem.Name, e.TopItem.Date, e.TopItem.Hits);

        }
    }
}
