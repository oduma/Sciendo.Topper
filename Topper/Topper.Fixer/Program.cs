using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sciendo.Config;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Store;
using Serilog;

namespace Topper.Fixer
{
    class Program
    {
        static int Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            var serviceProvider = ConfigureLog(serviceCollection);
            var logger = serviceProvider.GetService<ILogger<Program>>();

            logger.LogInformation("Starting...");

            var topperFixerConfig = ReadConfiguration(logger, args);
            serviceProvider = ConfigureServices(serviceCollection, topperFixerConfig);


            var currentDate = topperFixerConfig.StartFrom.AddDays(1);
            using (var itemsRepo = serviceProvider.GetService<IRepository<TopItem>>())
            {
                try
                {
                    itemsRepo.OpenConnection();
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Timeout while creating database and collection.");
                    throw;
                }
              
                while (currentDate < DateTime.Today)
                {
                    var dayTopItems = itemsRepo.GetItemsAsync(t => t.Date == currentDate).Result.ToList();
                    CalculateStoreTodayItems(logger, serviceProvider,dayTopItems,serviceProvider.GetService<IStoreManager>());

                }
            }
            return 0;
        }

        public delegate RuleBase RulesResolver(string key);

        private static ServiceProvider ConfigureServices(ServiceCollection serviceCollection, TopperFixerConfig topperFixerConfig)
        {
            serviceCollection.AddTransient<IRepository<TopItem>>(r => new Repository<TopItem>(r.GetRequiredService<ILogger<Repository<TopItem>>>(), topperFixerConfig.CosmosDbConfig));
            serviceCollection.AddTransient<IStoreManager, StoreManager>();
            serviceCollection.AddTransient<IRulesEngine, RulesEngine>();
            serviceCollection.AddTransient<ArtistScoreRule>(a => new ArtistScoreRule(a.GetRequiredService<ILogger<ArtistScoreRule>>(), a.GetRequiredService<IRepository<TopItem>>(), topperFixerConfig.TopperRulesConfig.RankingBonus));
            serviceCollection.AddTransient<LovedRule>(l => new LovedRule(l.GetRequiredService<ILogger<LovedRule>>(), l.GetRequiredService<IRepository<TopItem>>(), topperFixerConfig.TopperRulesConfig.LovedBonus));
            serviceCollection.AddTransient<RulesResolver>(serviceProvider => key =>
            {
                switch (key)
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

        private static void CalculateStoreTodayItems(ILogger<Program> logger, ServiceProvider serviceProvider,
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
                    storeLogic.UpdateItems(new[] { todayTopItem });
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Cannot persist item in store.");
                    throw;
                }
            }
        }

        private static TopperFixerConfig ReadConfiguration(Microsoft.Extensions.Logging.ILogger logger, string[] args)
        {
            var config =
                new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"{AppDomain.CurrentDomain.FriendlyName}.json")
                    .AddCommandLine(args)
                    .Build();
            TopperFixerConfig topperFixerConfig;
            try
            {
                topperFixerConfig = new ConfigurationManager<TopperFixerConfig>().GetConfiguration(config);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Something happened here!");
                throw;
            }

            return topperFixerConfig;
        }

        private static ServiceProvider ConfigureLog(IServiceCollection services)
        {
            return services.AddLogging(configure => configure.AddSerilog(new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger())).BuildServiceProvider();
        }
    }
}
