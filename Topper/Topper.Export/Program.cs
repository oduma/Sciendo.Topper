using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sciendo.Config;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Store;
using Serilog;

namespace Topper.Export
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            var serviceProvider = ConfigureLog(serviceCollection);
            var logger = serviceProvider.GetService<ILogger<Program>>();

            TopperExportConfig topperExportConfig = ReadConfiguration(logger, args);
            serviceProvider = ConfigureServices(serviceCollection, topperExportConfig);

            ExportData(logger,serviceProvider,topperExportConfig);
        }

        private static ServiceProvider ConfigureServices(ServiceCollection serviceCollection, TopperExportConfig topperExportConfig)
        {
            serviceCollection.AddTransient<IRepository<TopItem>>(r => new Repository<TopItem>(r.GetRequiredService<ILogger<Repository<TopItem>>>(), topperExportConfig.CosmosDbConfig));

            return serviceCollection.BuildServiceProvider();
        }

        private static TopperExportConfig ReadConfiguration(ILogger<Program> logger, string[] args)
        {
            try
            {
                    return new ConfigurationManager<TopperExportConfig>().GetConfiguration(new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("topper.export.json")
                        .AddCommandLine(args)
                        .Build());
            }
            catch (Exception e)
            {
                logger.LogError(e,"wrong config!");
                throw e;
            }
        }

        private static ServiceProvider ConfigureLog(ServiceCollection services)
        {
            return services.AddLogging(configure => configure.AddSerilog(new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger())).BuildServiceProvider();
        }

        private static void ExportData(ILogger<Program> logger, ServiceProvider serviceProvider, TopperExportConfig topperExportConfig)
        {
            int i = 0;
            using (var itemsRepository = serviceProvider.GetService<IRepository<TopItem>>())
            {
                using (var fs = File.CreateText(topperExportConfig.OutputFile))
                {
                    fs.WriteLine("Artist,Day,Hits,NoOfLoved,Score,Year,DayRanking");
                    foreach (var topItem in itemsRepository.GetAllItemsAsync().Result)
                    {
                        fs.WriteLine(
                            $"{topItem.Name},{topItem.Date.Day}/{topItem.Date.Month}/{topItem.Date.Year},{topItem.Hits},{topItem.Loved},{topItem.Score},{topItem.Year},{topItem.DayRanking}");
                        logger.LogInformation("Exported {1} - {0} Ok.",topItem.Name,topItem.Date);
                        i++;
                    }
                    fs.Flush();
                }
            }
            logger.LogInformation("Written {0} items in the file {1}", i, topperExportConfig.OutputFile);
        }
    }
}
