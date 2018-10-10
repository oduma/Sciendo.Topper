using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Sciendo.Config;
using Sciendo.Topper.Contracts;
using Sciendo.Topper.Store;

namespace Topper.DataMigration
{
    class Program
    {
        static void Main(string[] args)
        {
            var config =
                new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("topper.datamigration.json")
                    .AddCommandLine(args)
                    .Build();

            TopperDataMigrationConfig topperDataMigrationConfig =
                new ConfigurationManager<TopperDataMigrationConfig>().GetConfiguration(config);
            using (var itemsRepository = new Repository<TopItem>(topperDataMigrationConfig.CosmosDb))
            {
                int i = 0;
                var storeLogic = new StoreLogic();
                storeLogic.Progress += StoreLogic_Progress;
                foreach (var topItem in ReadFile(topperDataMigrationConfig.InoutFile))
                {
                    try
                    {
                        storeLogic.StoreItem(topItem, itemsRepository);
                        Console.WriteLine($"Saved {i++} documents {topItem.Name}.");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }

        private static void StoreLogic_Progress(object sender, ProgressEventArgs e)
        {
            Console.WriteLine("{0} - {1}.",e.Status,e.TopItem.Name);
        }

        private static IEnumerable<TopItem> ReadFile(string inputFile)
        {
            if(!File.Exists(inputFile))
                throw new IOException($"{inputFile} does not exist.");
            foreach (var fileLine in File.ReadLines(inputFile))
            {
                if(fileLine.StartsWith("Artist,Day,Hits,TempScore,NoOfLoved,Score"))
                    continue;
                else
                {
                    var fileLineParts = fileLine.Split(new char[] {','}, StringSplitOptions.None);
                    if(!string.IsNullOrEmpty(fileLineParts[0]))
                        yield return
                            new TopItem
                            {
                                Name = fileLineParts[0],
                                Date = Convert.ToDateTime(fileLineParts[1]),
                                Hits = Convert.ToInt32(fileLineParts[2]),
                                Score = Convert.ToInt32(fileLineParts[5]),
                                Loved=(string.IsNullOrEmpty(fileLineParts[4]))?0:Convert.ToInt32(fileLineParts[4])
                            };
                }
            }
        }
    }
}
