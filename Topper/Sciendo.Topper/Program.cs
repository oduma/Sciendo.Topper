using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Sciendo.Topper.Source;
using Sciendo.Last.Fm;
using Sciendo.Topper.Contracts;
using Sciendo.Topper.Store;

namespace Sciendo.Topper
{
    class Program
    {
        static void Main(string[] args)
        {
            var config =
                new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("topper.json")
                    .Build();
            var cosmosDb = config.GetSection("cosmosDb").Get<CosmosDb>();
            var topperLastFmConfig = config.GetSection("topperLastFm").Get<TopperLastFmConfig>();
            var topperConfig = config.GetSection("topper").Get<TopperConfig>();
            var lastFmTopArtistSourcer = new LastFmTopArtistSourcer(new ContentProvider<RootObject>( new UrlProvider(topperLastFmConfig.ApiKey),new LastFmProvider()));
            var topItems = lastFmTopArtistSourcer.GetTopItems(topperLastFmConfig.UserName);
            var storeLogic = new StoreLogic();
            storeLogic.Progress += StoreLogic_Progress;
            using (var itemsRepo = new Repository<TopItemWithScore>(cosmosDb))
            {
                storeLogic.StoreItems(topItems,itemsRepo,topperConfig.Bonus);
            }
            Console.ReadKey();
        }

        private static void StoreLogic_Progress(object sender, ProgressEventArgs e)
        {
            Console.WriteLine("{0} item {1}; {2}; {3}.", e.Status, e.TopItem.Name, e.TopItem.Date, e.TopItem.Hits);

        }
    }
}
