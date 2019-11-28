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


                List<TopItem> yearAggregate = new List<TopItem>();

                using (var itemsRepo = serviceProvider.GetService<IRepository<TopItem>>())
                {
                    try
                    {
                        itemsRepo.OpenConnection();
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e,"Timeout while creating database and collection.");
                        throw;
                    }

                    CalculateTopItemsAndGetAggregateItems(logger, serviceProvider, todayTopItems, yearAggregate);
                }

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
            serviceCollection.AddTransient<IRepository<TopItem>>(r => new Repository<TopItem>(r.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Repository<TopItem>>>(), topperConfig.CosmosDbConfig));
            serviceCollection.AddTransient<IEmailSender>(e => new EmailSender(e.GetRequiredService <ILogger<EmailSender>>(), topperConfig.EmailOptions));
            serviceCollection.AddTransient<INotificationManager>(n => new NotificationManager(n.GetRequiredService<ILogger<NotificationManager>>(), n.GetRequiredService<IEmailSender>(),"mail"));
            serviceCollection.AddTransient<IUrlProvider>(u => new UrlProvider(u.GetRequiredService<ILogger<UrlProvider>>(), topperConfig.TopperLastFmConfig.ApiKey));
            serviceCollection.AddTransient<ILastFmProvider, LastFmProvider>();
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

        private static void CalculateTopItemsAndGetAggregateItems(Microsoft.Extensions.Logging.ILogger<Program> logger, 
            ServiceProvider serviceProvider, List<TopItem> todayTopItems,
            List<TopItem> yearAggregate)
        {
            var storeLogic = serviceProvider.GetService<IStoreManager>();
            storeLogic.Progress += StoreLogic_Progress;
            if (todayTopItems.Count > 0)
                CalculateStoreTodayItems(logger, serviceProvider, todayTopItems, storeLogic);
            else
            {
                logger.LogWarning("No top items for today no calculation of scores needed.");
            }

            yearAggregate.AddRange(storeLogic.GetAggregateHistoryOfScores());
            if (!yearAggregate.Any())
            {
                logger.LogWarning("No year aggregate retrieve.");
            }
            else
                logger.LogInformation("Items scored this year {0}", yearAggregate.Count);
        }

        private static TopperConfig ReadConfiguration(Microsoft.Extensions.Logging.ILogger<Program> logger, string[] args)
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

        private static void CalculateStoreTodayItems(Microsoft.Extensions.Logging.ILogger<Program> logger, 
            ServiceProvider serviceProvider, 
            List<TopItem> todayTopItems,
            IStoreManager storeLogic)
        {
            logger.LogInformation("Calculating scores for today's items...");
            var rulesEngine = serviceProvider.GetService<IRulesEngine>();
            rulesEngine.AddRule(serviceProvider.GetService<RulesResolver>()("TOPARTIST"));
            rulesEngine.AddRule(serviceProvider.GetService<RulesResolver>()("LOVEDTRACK"));
            foreach (var todayTopItem in todayTopItems)
            {
                logger.LogInformation("Calculating Score for {0}", todayTopItem.Name);
                rulesEngine.ApplyAllRules(todayTopItem);
                try
                {
                    storeLogic.StoreItem(todayTopItem);
                }
                catch (Exception e)
                {
                    logger.LogError(e,"Cannot persist item in store.");
                    throw;
                }
            }
        }

        private static List<TopItem> GetTodayTopItems(Microsoft.Extensions.Logging.ILogger<Program> logger,  ServiceProvider serviceProvider,
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
