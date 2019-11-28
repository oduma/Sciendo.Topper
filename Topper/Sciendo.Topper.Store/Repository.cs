using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;

namespace Sciendo.Topper.Store
{
    public class Repository<T>: IRepository<T>
    {
        private readonly CosmosDbConfig _cosmosDbConfig;
        private readonly DocumentClient _documentClient;
        private ILogger<Repository<T>> _logger;

        public Repository (ILogger<Repository<T>> logger, CosmosDbConfig cosmosDbConfig)
        {
            _logger = logger;
            _logger.LogInformation(
                "Creating a repository for Database:{0} and collection: {1}",
                cosmosDbConfig.DatabaseId, cosmosDbConfig.CollectionId);
            _cosmosDbConfig = cosmosDbConfig;
            _documentClient = new DocumentClient(new Uri(_cosmosDbConfig.Endpoint), _cosmosDbConfig.Key,
                new ConnectionPolicy {EnableEndpointDiscovery = false});
            _logger.LogInformation("Repository created.");

        }
        public void Dispose()
        {
            _documentClient.Dispose();
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await _documentClient.ReadDocumentCollectionAsync(
                    UriFactory.CreateDocumentCollectionUri(_cosmosDbConfig.DatabaseId, _cosmosDbConfig.CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _documentClient.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(_cosmosDbConfig.DatabaseId),
                        new DocumentCollection { Id = _cosmosDbConfig.CollectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await _documentClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_cosmosDbConfig.DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _documentClient.CreateDatabaseAsync(new Database { Id = _cosmosDbConfig.DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<Document> CreateItemAsync(T item)
        {
            
            return
                await _documentClient.CreateDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(_cosmosDbConfig.DatabaseId, _cosmosDbConfig.CollectionId), item);
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            IDocumentQuery<T> query = _documentClient.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(_cosmosDbConfig.DatabaseId, _cosmosDbConfig.CollectionId),
                new FeedOptions { MaxItemCount = -1,EnableCrossPartitionQuery=true })
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<IEnumerable<T>> GetAllItemsAsync()
        {
            IDocumentQuery<T> query = _documentClient.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_cosmosDbConfig.DatabaseId, _cosmosDbConfig.CollectionId),
                    new FeedOptions { MaxItemCount = -1,EnableCrossPartitionQuery=true })
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public void OpenConnection()
        {
            _logger.LogInformation(
                "Opening a connection to the database.");

            _documentClient.OpenAsync();
            if (!CreateDatabaseIfNotExistsAsync().Wait(1000))
            {
                //throw new Exception("Timeout while tring to create the database.");
            }

            if (CreateCollectionIfNotExistsAsync().Wait(1000))
            {
                //throw new Exception("Timeout while trying to create the collection.");
            }
            _logger.LogInformation(
                "Connection to the database opened Ok.");

        }
    }
}
