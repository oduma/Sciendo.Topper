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

namespace Topper.DataMigration
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            var serviceProvider = ConfigureLog(serviceCollection);
            var logger = serviceProvider.GetService<ILogger<Program>>();

            TopperDataMigrationConfig topperDataMigrationConfig = ReadConfiguration(logger, args);
            serviceProvider = ConfigureServices(serviceCollection, topperDataMigrationConfig);

            MigrateData(logger, serviceProvider, topperDataMigrationConfig);
        }

        private static ServiceProvider ConfigureServices(ServiceCollection serviceCollection, TopperDataMigrationConfig topperDataMigrationConfig)
        {
            serviceCollection.AddSingleton<IRepository<TopItem>>((p)=> { return new Repository<TopItem>(p.GetRequiredService<ILogger<Repository<TopItem>>>(), topperDataMigrationConfig.CosmosDbConfig); });
            serviceCollection.AddTransient<IStoreManager, StoreManager>();
            return serviceCollection.BuildServiceProvider();

        }

        private static TopperDataMigrationConfig ReadConfiguration(ILogger<Program> logger, string[] args)
        {
            try
            {
                return new ConfigurationManager<TopperDataMigrationConfig>().GetConfiguration(new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("topper.dataMigration.json")
                        .AddCommandLine(args)
                        .Build());
            }
            catch (Exception e)
            {
                logger.LogError("Error in Configuration.",e);
                throw e;
            }

        }

        private static ServiceProvider ConfigureLog(IServiceCollection services)
        {
            return services.AddLogging(configure => configure.AddSerilog(new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger())).BuildServiceProvider();
        }

        private static void MigrateData(Microsoft.Extensions.Logging.ILogger logger, ServiceProvider serviceProvider, TopperDataMigrationConfig topperDataMigrationConfig)
        {
            using (var itemsRepository = serviceProvider.GetService<IRepository<TopItem>>())
            {
                itemsRepository.OpenConnection();
                int i = 0;
                var storeLogic = serviceProvider.GetService<IStoreManager>();
                storeLogic.Progress += StoreLogic_Progress;
                IEnumerable<TopItem> dataToMigrate;
                try
                {
                    dataToMigrate = ReadFile(topperDataMigrationConfig.InoutFile);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "");
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
