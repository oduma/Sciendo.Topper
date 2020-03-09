using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sciendo.Config;
using Sciendo.Topper.Contracts;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using Sciendo.Topper.Service;
using Sciendo.Topper.Store;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace OverallCalculator
{
    class Program
    {
        static void Main(string[] args)
        {

            var serviceCollection = new ServiceCollection();
            var serviceProvider = ConfigureLog(serviceCollection);
            var logger = serviceProvider.GetService<ILogger<Program>>();
            var overallCalculatorConfig = ReadConfiguration(logger,args);
            
            CalculateAllDataAndUpdate(logger, 
                ConfigureServices(serviceCollection, overallCalculatorConfig), 
                overallCalculatorConfig.StartDate);

        }

        private static void CalculateAllDataAndUpdate(ILogger<Program> logger,
            ServiceProvider serviceProvider, 
            string startDate)
        {
            logger.LogInformation("Starting ...");
            using(var dailyItemsRepository = serviceProvider.GetService<IRepository<TopItem>>())
            {
                var overallStoreManager = serviceProvider.GetService<IOverallStoreManager>();
                var storeManager = serviceProvider.GetService<IStoreManager>();
                foreach (var year in dailyItemsRepository
                .GetAllItemsAsync()
                .Result
                .Select(i => i.Year)
                .Distinct()
                .OrderBy(y => y))
                {
                    logger.LogInformation("Calculating overall totals for year: {0}", year);
                    int previousTotal = 0;

                    DateTime? possibleStartingDate = GetStartingDate(startDate, overallStoreManager);
                    Expression<Func<TopItem, bool>> predicate = GetDateFilteringPredicate(year, possibleStartingDate);
                    if (possibleStartingDate.HasValue && possibleStartingDate.Value.Year != Convert.ToInt32(year))
                        continue;

                    foreach (var day in dailyItemsRepository
                        .GetItemsAsync(predicate)
                        .Result
                        .Select(i => i.Date)
                        .Distinct()
                        .OrderBy(d => d))
                    {
                        logger.LogInformation("Calculating overall totals for day: {0}", day);
                        var dailyChanges = dailyItemsRepository
                            .GetItemsAsync(i => i.Year == year && i.Date == day)
                            .Result
                            .ToArray();
                        int currentTotal = 0;
                        dailyChanges = overallStoreManager.AdvanceOverallItems(day, dailyChanges, out currentTotal);
                        if (previousTotal <= currentTotal)
                            previousTotal = currentTotal;
                        else
                            logger.LogError("Something happened with {0} previous total: {1} current total: {2}", day, previousTotal, currentTotal);
                        storeManager.UpdateItems(dailyChanges);
                    }
                }
            }
        }

        private static Expression<Func<TopItem, bool>> GetDateFilteringPredicate(string year, DateTime? possibleStartingDate)
        {
            Expression<Func<TopItem, bool>> predicate = null;
            if (possibleStartingDate.HasValue)
            {
                predicate = (i) => i.Year == year && i.Date >= possibleStartingDate.Value;
            }
            else
            {
                predicate = (i) => i.Year == year;
            }

            return predicate;
        }

        private static DateTime? GetStartingDate(string startDate, IOverallStoreManager overallStoreManager)
        {
            DateTime? possibleStartingDate = null;
            if (string.IsNullOrEmpty(startDate))
            {
                possibleStartingDate = overallStoreManager.NextStartingDate();
            }
            else
            {
                possibleStartingDate = startDate.ToDate();
            }

            return possibleStartingDate;
        }

        private static ServiceProvider ConfigureServices(ServiceCollection serviceCollection, OverallCalculatorConfig overallCalculatorConfig)
        {
            if (overallCalculatorConfig == null)
                throw new ArgumentNullException(nameof(overallCalculatorConfig));
            serviceCollection.AddSingleton(overallCalculatorConfig.CosmosDbConfig);
            serviceCollection.AddSingleton<IRepository<TopItem>>(r => new Repository<TopItem>(r.GetRequiredService<ILogger<Repository<TopItem>>>(), overallCalculatorConfig.CosmosDbConfig,overallCalculatorConfig.CurrentSourceCollectionId));
            serviceCollection.AddSingleton<IRepository<TopItemWithPartitionKey>>(r => new Repository<TopItemWithPartitionKey>(r.GetRequiredService<ILogger<Repository<TopItemWithPartitionKey>>>(), overallCalculatorConfig.CosmosDbConfig,overallCalculatorConfig.CurrentTargetCollectionId));
            serviceCollection.AddTransient<IMapper<TopItem, TopItemWithPartitionKey>, MapTopItemToTopItemWithPartitionKey>();
            serviceCollection.AddTransient<IOverallStoreManager, OverallStoreManager>();
            serviceCollection.AddTransient<IStoreManager,StoreManager>();
            return serviceCollection.BuildServiceProvider();
        }

        private static OverallCalculatorConfig ReadConfiguration(ILogger<Program> logger, string[] args)
        {
            try
            {
                return new ConfigurationManager<OverallCalculatorConfig>().GetConfiguration(new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"{AppDomain.CurrentDomain.FriendlyName}.json")
                    .AddCommandLine(args)
                    .Build());
            }
            catch (Exception e)
            {
                logger.LogError(e, "wrong config!");
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
    }
}
