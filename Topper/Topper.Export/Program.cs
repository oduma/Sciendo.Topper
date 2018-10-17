using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Sciendo.Config;
using Sciendo.Topper.Contracts;
using Sciendo.Topper.Store;

namespace Topper.Export
{
    class Program
    {
        static void Main(string[] args)
        {
            TopperExportConfig topperExportConfig;
            try
            {
                topperExportConfig =
                    new ConfigurationManager<TopperExportConfig>().GetConfiguration(new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("topper.export.json")
                        .AddCommandLine(args)
                        .Build());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            ExportData(topperExportConfig);
        }

        private static void ExportData(TopperExportConfig topperExportConfig)
        {
            int i = 0;
            using (var itemsRepository = new Repository<TopItem>(topperExportConfig.CosmosDbConfig))
            {
                using (var fs = File.CreateText(topperExportConfig.OutputFile))
                {
                    fs.WriteLine("Artist,Day,Hits,TempScore,NoOfLoved,Score");
                    foreach (var topItem in itemsRepository.GetAllItemsAsync().Result)
                    {
                        fs.WriteLine($"{topItem.Name},{topItem.Date.Day}/{topItem.Date.Month}/{topItem.Date.Year},{topItem.Hits},0,{topItem.Loved},{topItem.Score}");
                        Console.WriteLine("Exported {0} Ok.",topItem.Name);
                        i++;
                    }
                    fs.Flush();
                }
            }
            Console.WriteLine("Written {0} items in the file {1}", i, topperExportConfig.OutputFile);
        }
    }
}
