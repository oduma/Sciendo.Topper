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
            TopperDataMigrationConfig topperDataMigrationConfig;
            try
            {
                topperDataMigrationConfig =
                    new ConfigurationManager<TopperDataMigrationConfig>().GetConfiguration(new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("topper.dataMigration.json")
                        .AddCommandLine(args)
                        .Build());
            }
            catch (Exception e)
            {
                    Console.WriteLine(e);
                    throw;
            }
            MigrateData(topperDataMigrationConfig);
        }

        private static void MigrateData(TopperDataMigrationConfig topperDataMigrationConfig)
        {
            using (var itemsRepository = new Repository<TopItem>(topperDataMigrationConfig.CosmosDbConfig))
            {
                int i = 0;
                var storeLogic = new StoreManager(itemsRepository);
                storeLogic.Progress += StoreLogic_Progress;
                IEnumerable<TopItem> dataToMigrate;
                try
                {
                    dataToMigrate = ReadFile(topperDataMigrationConfig.InoutFile);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return;
                }
                foreach (var topItem in dataToMigrate)
                {
                    try
                    {
                        storeLogic.StoreItem(topItem);
                        Console.WriteLine($"Saved {i++} documents {topItem.Name}.");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return;
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
                if(fileLine.StartsWith("Artist,Day,Hits,NoOfLoved,Score,Year,DayRanking"))
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
                                Loved = Convert.ToInt32(fileLineParts[3]),
                                Score = Convert.ToInt32(fileLineParts[4]),
                                Year=fileLineParts[5],
                                DayRanking = Convert.ToInt32(fileLineParts[6])
                            };
                }
            }
        }
    }
}
