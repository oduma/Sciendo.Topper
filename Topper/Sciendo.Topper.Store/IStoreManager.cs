using System;
using System.Collections.Generic;
using Sciendo.Topper.Domain;

namespace Sciendo.Topper.Store
{
    public interface IStoreManager
    {
        event EventHandler<ProgressEventArgs> Progress;

        IEnumerable<TopItem> GetAggregateHistoryOfScores();
        void StoreItem(TopItem topItem);
    }
}