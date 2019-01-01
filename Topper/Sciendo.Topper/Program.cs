using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Sciendo.Config;
using Sciendo.Last.Fm;
using Sciendo.Topper.Source;
using Sciendo.Topper.Contracts;
using Sciendo.Topper.Notifier;
using Sciendo.Topper.Source.DataTypes;
using Sciendo.Topper.Store;

namespace Sciendo.Topper
{
    class Program
    {
        static int Main(string[] args)
        {

            var config =
                new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("topper.json")
                    .AddCommandLine(args)
                    .Build();
            TopperConfig topperConfig;
            try
            {
               topperConfig= new ConfigurationManager<TopperConfig>().GetConfiguration(config);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var notifier = CreateNotifier(topperConfig.EmailOptions);

            if (!notifier.SendPreviousFailedEmails())
            {
                var todayTopItems = GetTodayTopItems(topperConfig.TopperLastFmConfig);


                List<TopItem> yearAggregate = new List<TopItem>();

                using (var itemsRepo = new Repository<TopItem>(topperConfig.CosmosDbConfig))
                {
                    var storeLogic = new StoreManager(itemsRepo);
                    storeLogic.Progress += StoreLogic_Progress;

                    CalculateStoreTodayItems(itemsRepo, topperConfig.TopperRulesConfig, todayTopItems, storeLogic);

                    yearAggregate.AddRange(storeLogic.GetAggregateHistoryOfScores());
                }

                if (notifier.ComposeAndSendMessage(todayTopItems, yearAggregate, topperConfig.DestinationEmail))
                {
                    Console.WriteLine("check your email");
                    return 0;
                }
                else
                {
                    return -1;
                }
            }

            return 0;
        }

        private static void CalculateStoreTodayItems(Repository<TopItem> itemsRepo, TopperRulesConfig topperRulesConfig, 
            List<TopItem> todayTopItems,
            StoreManager storeLogic)
        {
            var rulesEngine = new RulesEngine();
            rulesEngine.AddRule(new ArtistScoreRule(itemsRepo, topperRulesConfig.RankingBonus));
            rulesEngine.AddRule(new LovedRule(itemsRepo, topperRulesConfig.LovedBonus));
            foreach (var todayTopItem in todayTopItems)
            {
                rulesEngine.ApplyAllRules(todayTopItem);
                try
                {
                    storeLogic.StoreItem(todayTopItem);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private static List<TopItem> GetTodayTopItems(LastFmConfig topperLastFmConfig)
        {
            List<TopItem> todayTopItems;
            IUrlProvider urlProvider;
            try
            {
                urlProvider = new UrlProvider(topperLastFmConfig.ApiKey);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
                Console.WriteLine(e);
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
                Console.WriteLine(e);
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
                Console.WriteLine(e);
                throw;
            }

            return todayTopItems;
        }

        private static NotificationManager CreateNotifier(EmailConfig emailConfig)
        {
            NotificationManager notifier;
            try
            {
                notifier = new NotificationManager(new EmailSender(emailConfig),
                    emailConfig.NotSendFileExtension);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return notifier;
        }

        private static void StoreLogic_Progress(object sender, ProgressEventArgs e)
        {
            Console.WriteLine("{0} item {1}; {2}; {3}.", e.Status, e.TopItem.Name, e.TopItem.Date, e.TopItem.Hits);

        }
    }
}
