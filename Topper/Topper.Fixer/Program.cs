using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Sciendo.Config;
using Sciendo.Topper.Contracts;
using Sciendo.Topper.Store;
using Serilog;

namespace Topper.Fixer
{
    class Program
    {
        static int Main(string[] args)
        {
            ConfigureLog();
            Log.Information("Starting...");

            var topperFixerConfig = ReadConfiguration(args);


            var currentDate = topperFixerConfig.StartFrom.AddDays(1);
            using (var itemsRepo = new Repository<TopItem>(topperFixerConfig.CosmosDbConfig))
            {
                try
                {
                    itemsRepo.OpenConnection();
                }
                catch (Exception e)
                {
                    Log.Error(e, "Timeout while creating database and collection.");
                    throw;
                }
              
                while (currentDate < DateTime.Today)
                {
                    var dayTopItems = itemsRepo.GetItemsAsync(t => t.Date == currentDate).Result.ToList();
                    CalculateStoreTodayItems(itemsRepo,topperFixerConfig.TopperRulesConfig,dayTopItems, new StoreManager(itemsRepo));

                }
            }
            return 0;
        }

        private static void CalculateStoreTodayItems(Repository<TopItem> itemsRepo, TopperRulesConfig topperRulesConfig,
            List<TopItem> todayTopItems,
            StoreManager storeLogic)
        {
            Log.Information("Calculating scores for today's items...");
            var rulesEngine = new RulesEngine();
            rulesEngine.AddRule(new ArtistScoreRule(itemsRepo, topperRulesConfig.RankingBonus));
            rulesEngine.AddRule(new LovedRule(itemsRepo, topperRulesConfig.LovedBonus));
            foreach (var todayTopItem in todayTopItems)
            {
                Log.Information("Calculating Score for {0}", todayTopItem.Name);
                rulesEngine.ApplyAllRules(todayTopItem);
                try
                {
                    storeLogic.StoreItem(todayTopItem);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Cannot persist item in store.");
                    throw;
                }
            }
        }

        private static TopperFixerConfig ReadConfiguration(string[] args)
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
                Log.Error(e, "Something happened here!");
                throw;
            }

            return topperFixerConfig;
        }

        private static void ConfigureLog()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
