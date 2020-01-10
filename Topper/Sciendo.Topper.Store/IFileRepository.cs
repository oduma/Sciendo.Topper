using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Sciendo.Topper.Store
{
    public interface IFileRepository<T>
    {
        string CreateItem(T item);

        string GetItem(string artistName);

    }
}
