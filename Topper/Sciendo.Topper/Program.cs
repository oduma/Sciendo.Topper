using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Sciendo.Config;
using Sciendo.Last.Fm;
using Sciendo.Topper.Source;
using Sciendo.Topper.Contracts;
using Sciendo.Topper.Notifier;
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
            TopperConfig topperConfig = new ConfigurationManager<TopperConfig>().GetConfiguration(config);

            var topItemsSourcer=new TopItemsSourcer();
            topItemsSourcer.RegisterSourcer(new LastFmTopArtistSourcer(
                new ContentProvider<TopArtistsRootObject>(new UrlProvider(topperConfig.TopperLastFmConfig.ApiKey),
                    new LastFmProvider())));
            topItemsSourcer.RegisterSourcer(new LastFmLovedSourcer(
                new ContentProvider<LovedTracksRootObject>(new UrlProvider(topperConfig.TopperLastFmConfig.ApiKey),
                    new LastFmProvider())));

            var todayTopItems = topItemsSourcer.GetItems(topperConfig.TopperLastFmConfig.UserName);

            var notifier = new NotificationCreator(new EmailSender(topperConfig.EmailOptions),
                topperConfig.EmailOptions.NotSendFileExtension);

            notifier.SendPreviousFailedEmails();
            var storeLogic = new StoreLogic();
            storeLogic.Progress += StoreLogic_Progress;
            IEnumerable<TopItem> yearAggregate;

            using (var itemsRepo = new Repository<TopItem>(topperConfig.CosmosDb))
            {
                var rulesEngine = new RulesEngine();
                rulesEngine.AddRule(new ArtistScoreRule(itemsRepo,topperConfig.TopperRulesConfig.RankingBonus));
                rulesEngine.AddRule(new LovedRule(itemsRepo,topperConfig.TopperRulesConfig.LovedBonus));
                foreach (var todayTopItem in todayTopItems)
                {
                    rulesEngine.ApplyAllRules(todayTopItem);
                    storeLogic.StoreItem(todayTopItem,itemsRepo);
                }
                yearAggregate = storeLogic.GetAggregateHistoryOfScores(itemsRepo, 0);
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

        private static void StoreLogic_Progress(object sender, ProgressEventArgs e)
        {
            Console.WriteLine("{0} item {1}; {2}; {3}.", e.Status, e.TopItem.Name, e.TopItem.Date, e.TopItem.Hits);

        }
    }
}
