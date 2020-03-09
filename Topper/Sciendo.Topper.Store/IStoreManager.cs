using System;
using System.Collections.Generic;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;

namespace Sciendo.Topper.Store
{
    public interface IStoreManager
    {
        IEnumerable<TopItem> GetLatestPreviousVersionOfItems(DateTime date, IEnumerable<TopItem> itemsForDate);

        IEnumerable<TopItem> GetItemsByDate(DateTime date);

        void UpdateItems(IEnumerable<TopItem> topItems);

        IEnumerable<TopItem> GetItemsForNames(IEnumerable<string> names);

        DateTimeInterval GetTimeInterval();
    }
}