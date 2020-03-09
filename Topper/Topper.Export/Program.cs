using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sciendo.Config;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using Sciendo.Topper.Store;
using Serilog;
using Topper.ImportExport;
using Topper.ImportExport.Export;
using Topper.ImportExport.Import;

namespace Topper.Export
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            var serviceProvider = ConfigureLog(serviceCollection);
            var logger = serviceProvider.GetService<ILogger<Program>>();

            TopperImportExportConfig topperImportExportConfig = ReadConfiguration(logger, args);

            serviceProvider = ConfigureServices(serviceCollection, topperImportExportConfig);

            if (topperImportExportConfig.OperationType == OperationType.Export)
                ExecuteExport(logger,serviceProvider, topperImportExportConfig);
            else if (topperImportExportConfig.OperationType == OperationType.Import)
                ExecuteImport(logger,serviceProvider, topperImportExportConfig);
        }

        private static void ExecuteImport(ILogger<Program> logger, ServiceProvider serviceProvider, TopperImportExportConfig topperImportExportConfig)
        {
            int noOfItemsImported = 0;

            if (topperImportExportConfig.CosmosDbConfig.CosmosDbCollections
                .FirstOrDefault(c => c.CollectionId == topperImportExportConfig.Collection).TypeOfItem == typeof(TopItem).Name)
            {
                var repository = serviceProvider.GetService<IRepository<TopItem>>();
                repository.OpenConnection();
                noOfItemsImported = new Importer<TopItem>(
                    serviceProvider.GetService<ILogger<Importer<TopItem>>>(),
                    repository
                    )
                    .Import(topperImportExportConfig.Csv,topperImportExportConfig.ImportTransformations);
            }

            if (topperImportExportConfig.CosmosDbConfig.CosmosDbCollections
                .FirstOrDefault(c => c.CollectionId == topperImportExportConfig.Collection).TypeOfItem == typeof(TopItemWithPartitionKey).Name)
            {
                var repository = serviceProvider.GetService<IRepository<TopItemWithPartitionKey>>();
                repository.OpenConnection();
                noOfItemsImported = new Importer<TopItemWithPartitionKey>(
                    serviceProvider.GetService<ILogger<Importer<TopItemWithPartitionKey>>>(),
                    repository)
                    .Import(topperImportExportConfig.Csv,topperImportExportConfig.ImportTransformations);
            }
            logger.LogInformation("Imported: {0} items in collection {1}", noOfItemsImported, topperImportExportConfig.Collection);
        }

        private static void ExecuteExport(ILogger<Program> logger, ServiceProvider serviceProvider, TopperImportExportConfig topperImportExportConfig)
        {
            int noOfItemsExported = 0;

            if (topperImportExportConfig.CosmosDbConfig.CosmosDbCollections
                .FirstOrDefault(c => c.CollectionId == topperImportExportConfig.Collection).TypeOfItem == typeof(TopItem).Name)
            {
                noOfItemsExported = new Exporter<TopItem>(
                    serviceProvider.GetService<ILogger<Exporter<TopItem>>>(),
                    serviceProvider.GetService<IRepository<TopItem>>())
                    .Export(topperImportExportConfig.Csv);
            }

            if (topperImportExportConfig.CosmosDbConfig.CosmosDbCollections
                .FirstOrDefault(c => c.CollectionId == topperImportExportConfig.Collection).TypeOfItem == typeof(TopItemWithPartitionKey).Name)
            {
                noOfItemsExported = new Exporter<TopItemWithPartitionKey>(
                    serviceProvider.GetService<ILogger<Exporter<TopItemWithPartitionKey>>>(),
                    serviceProvider.GetService<IRepository<TopItemWithPartitionKey>>())
                    .Export(topperImportExportConfig.Csv);
            }
            logger.LogInformation("Exported: {0} items from collection {1}", noOfItemsExported, topperImportExportConfig.Collection);
        }

        private static ServiceProvider ConfigureServices(ServiceCollection serviceCollection, TopperImportExportConfig topperImportExportConfig)
        {

            serviceCollection.AddSingleton<IRepository<TopItem>>(r => new Repository<TopItem>(r.GetRequiredService<ILogger<Repository<TopItem>>>(), topperImportExportConfig.CosmosDbConfig,topperImportExportConfig.Collection));
            serviceCollection.AddSingleton<IRepository<TopItemWithPartitionKey>>(r => new Repository<TopItemWithPartitionKey>(r.GetRequiredService<ILogger<Repository<TopItemWithPartitionKey>>>(), topperImportExportConfig.CosmosDbConfig,topperImportExportConfig.Collection));


            return serviceCollection.BuildServiceProvider();
        }

        private static TopperImportExportConfig ReadConfiguration(ILogger<Program> logger, string[] args)
        {
            try
            {
                    return new ConfigurationManager<TopperImportExportConfig>().GetConfiguration(new ConfigurationBuilder()
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
    }
}
