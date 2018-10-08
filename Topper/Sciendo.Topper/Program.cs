using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Sciendo.Config;
using Sciendo.Topper.Source;
using Sciendo.Last.Fm;
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
            Console.WriteLine("Config Loaded Ok!");
            var todayTopItems = TopItemsSourcer.GetTodayTopItems(topperConfig.TopperLastFmConfig);

            EmailSender emailSender= new EmailSender(topperConfig.EmailOptions);

            var notifier = new NotificationCreator(emailSender,topperConfig.EmailOptions.NotSendFileExtension);

            notifier.SendPreviousFailedEmails();
            var storeLogic = new StoreLogic();
            storeLogic.Progress += StoreLogic_Progress;
            IEnumerable<TopItemWithScore> yearAggregate;
            IEnumerable<TopItemWithScore> todayTopItemWithScores;
            using (var itemsRepo = new Repository<TopItemWithScore>(topperConfig.Cosmosdb))
            {
                todayTopItemWithScores = storeLogic.StoreItems(todayTopItems, itemsRepo,
                    topperConfig.TopperRulesConfig.RankingBonus, topperConfig.TopperRulesConfig.LovedBonus);
                yearAggregate = storeLogic.GetAggregateHistoryOfScores(itemsRepo, 0);
            }

            if (notifier.ComposeAndSendMessage(todayTopItemWithScores, yearAggregate, topperConfig.DestinationEmail))
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
