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


namespace Sciendo.Topper
{
    class Program
    {
        static int Main(string[] args)
        {
            ConfigureLog();
            Log.Information("Starting...");

            var topperConfig = ReadConfiguration(args);

            var notifier = CreateNotifier(topperConfig.EmailOptions);

            if (!notifier.SendPreviousFailedEmails())
            {
                Log.Information("Trying to send new email for today...");
                var todayTopItems = GetTodayTopItems(topperConfig.TopperLastFmConfig);


                List<TopItem> yearAggregate = new List<TopItem>();

                using (var itemsRepo = new Repository<TopItem>(topperConfig.CosmosDbConfig))
                {
                    try
                    {
                        itemsRepo.OpenConnection();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e,"Timeout while creating database and collection.");
                        throw;
                    }

                    CalculateTopItemsAndGetAggregateItems(itemsRepo, todayTopItems, topperConfig, yearAggregate);
                }

                if (!yearAggregate.Any() && !todayTopItems.Any())
                {
                    Log.Warning("Nothing to send. No email sent.");
                    return -2;
                }
                if (notifier.ComposeAndSendMessage(todayTopItems, yearAggregate, topperConfig.DestinationEmail))
                {
                    Log.Information("Check your email.");
                    return 0;
                }
                else
                {
                    Log.Warning("Email not sent.");
                    return -1;
                }
            }
            return 0;
        }

        private static void CalculateTopItemsAndGetAggregateItems(Repository<TopItem> itemsRepo, List<TopItem> todayTopItems,
            TopperConfig topperConfig, List<TopItem> yearAggregate)
        {
            var storeLogic = new StoreManager(itemsRepo);
            storeLogic.Progress += StoreLogic_Progress;
            if (todayTopItems.Count > 0)
                CalculateStoreTodayItems(itemsRepo, topperConfig.TopperRulesConfig, todayTopItems, storeLogic);
            else
            {
                Log.Warning("No top items for today no calculation of scores needed.");
            }

            yearAggregate.AddRange(storeLogic.GetAggregateHistoryOfScores());
            if (!yearAggregate.Any())
            {
                Log.Warning("No year aggregate retrieve.");
            }
            else
                Log.Information("Items scored this year {0}", yearAggregate.Count);
        }

        private static TopperConfig ReadConfiguration(string[] args)
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
                Log.Error(e, "Something happened here!");
                throw;
            }

            return topperConfig;
        }

        private static void ConfigureLog()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
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
                    Log.Error(e,"Cannot persist item in store.");
                    throw;
                }
            }
        }

        private static List<TopItem> GetTodayTopItems(LastFmConfig topperLastFmConfig)
        {
            Log.Information("Getting new top items from last.fm ...");
            List<TopItem> todayTopItems;
            IUrlProvider urlProvider;
            try
            {
                urlProvider = new UrlProvider(topperLastFmConfig.ApiKey);
            }
            catch (Exception e)
            {
                Log.Error(e,"Cannot use urlProvider...");
                throw;
            }

            LastFmTopArtistsProvider lastFmTopArtistsProvider;
            try
            {
                lastFmTopArtistsProvider = new LastFmTopArtistsProvider(
                    new ContentProvider<TopArtistsRootObject>(urlProvider,
                        new LastFmProvider()));
            }
            catch (Exception e)
            {
                Log.Error(e,"Cannot use LastFmTopArtistProvider");
                throw;
            }

            LastFmLovedProvider lastFmLovedProvider;
            try
            {
                lastFmLovedProvider = new LastFmLovedProvider(
                    new ContentProvider<LovedTracksRootObject>(urlProvider,
                        new LastFmProvider()));
            }
            catch (Exception e)
            {
                Log.Error(e,"Cannot use LastFmLovesProvider");
                throw;
            }

            var topItemsAggregator = new TopItemsAggregator();
            topItemsAggregator.RegisterProvider(lastFmTopArtistsProvider);
            topItemsAggregator.RegisterProvider(lastFmLovedProvider);

            try
            {
                todayTopItems = topItemsAggregator.GetItems(topperLastFmConfig.UserName);
            }
            catch (Exception e)
            {
                Log.Error(e,"Cannot Retrieve items from at least one source.");
                throw;
            }
            if(!todayTopItems.Any())
                Log.Warning("Todays top items not retrieved.");
            else
                Log.Information("Retrieved {0} items.", todayTopItems.Count);
            return todayTopItems;
        }

        private static NotificationManager CreateNotifier(EmailConfig emailConfig)
        {
            Log.Information("Creating notifier for {0} ...",emailConfig.Domain);
            NotificationManager notifier;
            try
            {
                notifier = new NotificationManager(new EmailSender(emailConfig),
                    emailConfig.NotSendFileExtension);
            }
            catch (Exception e)
            {
                Log.Error(e,"Cannot create notifier.");
                throw;
            }
            Log.Information("Notifier created.");
            return notifier;
        }

        private static void StoreLogic_Progress(object sender, ProgressEventArgs e)
        {
            Console.WriteLine("{0} item {1}; {2}; {3}.", e.Status, e.TopItem.Name, e.TopItem.Date, e.TopItem.Hits);

        }
    }
}
