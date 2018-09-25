﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
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
            var cosmoDb = config.GetSection("cosmosDb").Get<CosmosDb>();
            var topperConfig = config.GetSection("topper").Get<TopperRulesConfig>();
            var inputFile = config.GetValue<string>("InputFile");
            using (var itemsRepository = new Repository<TopItemWithScore>(cosmoDb))
            {
                int i = 0;
                var storeLogic = new StoreLogic();
                storeLogic.Progress += StoreLogic_Progress;
                foreach (var topItem in ReadFile(inputFile))
                {
                    var result = storeLogic.StoreItem(topItem, itemsRepository, topperConfig.Bonus);
                    if(result!=null)
                        Console.WriteLine($"Saved {i++} documents {result.Name}.");
                }
            }
                Console.ReadKey();
        }

        private static void StoreLogic_Progress(object sender, ProgressEventArgs e)
        {
            Console.WriteLine("{0} - {1}.",e.Status,e.TopItem.Name);
        }

        private static IEnumerable<TopItemWithScore> ReadFile(string inputFile)
        {
            if(!File.Exists(inputFile))
                throw new IOException($"{inputFile} does not exist.");
            foreach (var fileLine in File.ReadLines(inputFile))
            {
                if(fileLine.StartsWith("Artist,Day,Hits,"))
                    continue;
                else
                {
                    var fileLineParts = fileLine.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
                    yield return
                        new TopItemWithScore
                        {
                            Name = fileLineParts[0],
                            Date = Convert.ToDateTime(fileLineParts[1]),
                            Hits = Convert.ToInt32(fileLineParts[2]),
                            Score = Convert.ToInt32(fileLineParts[3])
                        };
                }
            }
        }
    }
}
