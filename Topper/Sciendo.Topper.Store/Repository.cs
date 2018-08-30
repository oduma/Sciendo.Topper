using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Sciendo.Topper.Store
{
    public class Repository<T>: IDisposable
    {
        private readonly CosmosDb _cosmosDb;
        private readonly DocumentClient _documentClient;

        public Repository (CosmosDb cosmosDb)
        {
            _cosmosDb = cosmosDb;
            _documentClient = new DocumentClient(new Uri(_cosmosDb.Endpoint), _cosmosDb.Key,
                new ConnectionPolicy {EnableEndpointDiscovery = false});
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();

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
                    UriFactory.CreateDocumentCollectionUri(_cosmosDb.DatabaseId, _cosmosDb.CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _documentClient.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(_cosmosDb.DatabaseId),
                        new DocumentCollection { Id = _cosmosDb.CollectionId },
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
                await _documentClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_cosmosDb.DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _documentClient.CreateDatabaseAsync(new Database { Id = _cosmosDb.DatabaseId });
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
                    UriFactory.CreateDocumentCollectionUri(_cosmosDb.DatabaseId, _cosmosDb.CollectionId), item);
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            IDocumentQuery<T> query = _documentClient.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(_cosmosDb.DatabaseId, _cosmosDb.CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

    }
}
