using System;
using System.Collections.Generic;
using System.Linq;
using Sciendo.Topper.Contracts;
using Serilog;

namespace Sciendo.Topper.Store
{
    public class StoreManager
    {
        public event EventHandler<ProgressEventArgs> Progress;


        private IRepository<TopItem> _itemsRepo;
        public StoreManager(IRepository<TopItem> itemsRepo)
        {
            _itemsRepo = itemsRepo ?? throw new ArgumentNullException(nameof(itemsRepo));
        }
        public void StoreItem(TopItem topItem)
        {
            if (topItem == null || topItem.Date == DateTime.MinValue || string.IsNullOrEmpty(topItem.Name) ||
                (topItem.Hits == 0 && topItem.Loved == 0))
                throw new ArgumentNullException(nameof(topItem));
            Log.Information("Persisting item {0}", topItem.Name);
            Progress?.Invoke(this, new ProgressEventArgs(topItem, Status.Pending));
            topItem.Date = new DateTime(topItem.Date.Year, topItem.Date.Month, topItem.Date.Day);

            var existingItem =
                _itemsRepo.GetItemsAsync(i => i.Name == topItem.Name && i.Date == topItem.Date)
                    .Result.FirstOrDefault();
            if (existingItem == null)
            {
                Log.Information("Item doesn't exist in store.");
                topItem.Year = topItem.Date.Year.ToString();
                _itemsRepo.CreateItemAsync(topItem);
                Progress?.Invoke(this, new ProgressEventArgs(topItem, Status.Created));
                Log.Information("Item persisted in store.");
            }
            else
            {
                Log.Warning("Item exists in store. Do nothing.");
                Progress?.Invoke(this, new ProgressEventArgs(existingItem, Status.Existing));
            }
        }

        public IEnumerable<TopItem> GetAggregateHistoryOfScores()
        {
            Log.Information("Aggregating scores from the beginning of year...");
            var result= _itemsRepo.GetItemsAsync((i) => i.Year == DateTime.Today.Year.ToString());
            return result.Result.GroupBy((i) => i.Name)
                .Select((t) => new TopItem { Name = t.Key, Score = t.Sum((v) => v.Score),Loved = t.Sum((l)=>l.Loved) })
                .OrderByDescending((t) => t.Score);
        }
    }
}
