using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace Sciendo.Topper.Store
{
    public interface IRepository<T> :IDisposable
    {
        Task<Document> UpsertItemAsync(T item);
        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> GetAllItemsAsync();

        void OpenConnection();
        T GetItem(Expression<Func<T, bool>> predicate);

        Task<Document> UpsertItemAsync(T item, string partitionKey);

        void DeleteItem(string documentId);

        void DeleteItem(string documentId, string partitionKey);
    }
}