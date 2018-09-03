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
            using (var itemsRepo = new Repository<TopItemWithScore>(cosmosDb))
            {
                foreach (var topItem in topItems)
                {
                    Console.WriteLine("{0} has {1} Hits as per:{2}.", topItem.Name, topItem.Hits, topItem.Date);
                    var existingItem =
                        itemsRepo.GetItemsAsync(i => i.Name == topItem.Name && i.Date == topItem.Date)
                            .Result.FirstOrDefault();
                    if (existingItem == null)
                    {
                        Console.WriteLine("Created with id {0}.",
                            itemsRepo.CreateItemAsync(RulesEngine.CalculateScoreForTopItem(topItem, itemsRepo, topperConfig.Bonus)));
                    }
                    else
                    {
                        Console.WriteLine($"Item allready exists for {existingItem.Name} and {existingItem.Date}");
                    }
                }
    
            }
            Console.ReadKey();
        }
    }
}
