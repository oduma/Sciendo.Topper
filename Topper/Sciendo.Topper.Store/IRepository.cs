using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace Sciendo.Topper.Store
{
    public interface IRepository<T> :IDisposable
    {
        Task<Document> CreateItemAsync(T item);
        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate);

//        Task<IEnumerable<T>> GetItemsAsync()
        Task<IEnumerable<T>> GetAllItemsAsync();

        void OpenConnection();
    }
}