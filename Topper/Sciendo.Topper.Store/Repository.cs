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
        private string currentCollectionId;

        public Repository (
            ILogger<Repository<T>> logger, 
            CosmosDbConfig cosmosDbConfig, 
            string currentCollectionId)
        {
            _logger = logger;

            _logger.LogInformation(
                "Creating a repository for Database:{0}",
                cosmosDbConfig.DatabaseId);
            this.currentCollectionId = currentCollectionId;
            _logger.LogInformation(
                "Current repository will target Collection:{0}",
                currentCollectionId);


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
                    UriFactory.CreateDocumentCollectionUri(_cosmosDbConfig.DatabaseId, currentCollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var documentCollection = new DocumentCollection { Id = currentCollectionId };
                    var collectionDefinition = _cosmosDbConfig.CosmosDbCollections.FirstOrDefault(c => c.CollectionId == currentCollectionId);
                    if (!string.IsNullOrEmpty(collectionDefinition.PartitionKeyName))
                    {
                        documentCollection.PartitionKey = new PartitionKeyDefinition();
                        documentCollection.PartitionKey.Paths.Add(collectionDefinition.PartitionKeyName);
                    }

                    await _documentClient.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(_cosmosDbConfig.DatabaseId),
                        documentCollection,
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

        public async Task<Document> UpsertItemAsync(T item)
        {
            
            return
                await _documentClient.UpsertDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(_cosmosDbConfig.DatabaseId, currentCollectionId), item);
        }

        public async Task<Document> UpsertItemAsync(T item, string partitionKey)
        {
            var requestOptions = new RequestOptions();
            requestOptions.PartitionKey = new PartitionKey(partitionKey);
            return
                await _documentClient.UpsertDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(_cosmosDbConfig.DatabaseId, currentCollectionId),
                    item,
                    requestOptions
                    );
        }
        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            IDocumentQuery<T> query = _documentClient.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(_cosmosDbConfig.DatabaseId, currentCollectionId),
                new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true})
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
                    UriFactory.CreateDocumentCollectionUri(_cosmosDbConfig.DatabaseId, currentCollectionId),
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
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
            _logger.LogInformation(
                "Connection to the database opened Ok.");

        }

        public T GetItem(Expression<Func<T, bool>> predicate)
        {
            return GetItemsAsync(predicate).Result.FirstOrDefault();
        }

        public void DeleteItem(string documentId)
        {
           
            _documentClient.DeleteDocumentAsync(
        UriFactory.CreateDocumentUri(_cosmosDbConfig.DatabaseId, currentCollectionId, documentId)).Wait();

        }

        public void DeleteItem(string documentId, string partitionKey)
        {
            var requestOptions = new RequestOptions();
            requestOptions.PartitionKey = new PartitionKey(partitionKey);

            _documentClient.DeleteDocumentAsync(
        UriFactory.CreateDocumentUri(_cosmosDbConfig.DatabaseId, currentCollectionId, documentId), requestOptions).Wait();
        }
    }
}
