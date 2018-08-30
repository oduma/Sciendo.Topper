using System;
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
            var inputFile = config.GetValue<string>("InputFile");
            using (var itemsRepository = new Repository<TopItemWithScore>(cosmoDb))
            {
                int i = 0;
                foreach (var topItem in ReadFile(inputFile))
                {
                    var result = itemsRepository.CreateItemAsync(topItem);
                    Console.WriteLine($"Saved {i++} documents.");
                }
            }
                Console.ReadKey();
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
