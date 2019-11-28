using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Sciendo.Topper.Domain;

namespace Sciendo.Topper.Store
{
    public class StoreManager : IStoreManager
    {
        public event EventHandler<ProgressEventArgs> Progress;

        private readonly ILogger<StoreManager> logger;
        private IRepository<TopItem> _itemsRepo;
        public StoreManager(ILogger<StoreManager> logger, IRepository<TopItem> itemsRepo)
        {
            this.logger = logger;
            _itemsRepo = itemsRepo ?? throw new ArgumentNullException(nameof(itemsRepo));
        }
        public void StoreItem(TopItem topItem)
        {
            if (topItem == null || topItem.Date == DateTime.MinValue || string.IsNullOrEmpty(topItem.Name) ||
                (topItem.Hits == 0 && topItem.Loved == 0))
                throw new ArgumentNullException(nameof(topItem));
            logger.LogInformation("Persisting item {0}", topItem.Name);
            Progress?.Invoke(this, new ProgressEventArgs(topItem, Status.Pending));
            topItem.Date = new DateTime(topItem.Date.Year, topItem.Date.Month, topItem.Date.Day);

            var existingItem =
                _itemsRepo.GetItemsAsync(i => i.Name == topItem.Name && i.Date == topItem.Date)
                    .Result.FirstOrDefault();
            if (existingItem == null)
            {
                logger.LogInformation("Item doesn't exist in store.");
                topItem.Year = topItem.Date.Year.ToString();
                _itemsRepo.CreateItemAsync(topItem);
                Progress?.Invoke(this, new ProgressEventArgs(topItem, Status.Created));
                logger.LogInformation("Item persisted in store.");
            }
            else
            {
                logger.LogWarning("Item exists in store. Do nothing.");
                Progress?.Invoke(this, new ProgressEventArgs(existingItem, Status.Existing));
            }
        }

        public IEnumerable<TopItem> GetAggregateHistoryOfScores()
        {
            logger.LogInformation("Aggregating scores from the beginning of year...");
            var result = _itemsRepo.GetItemsAsync((i) => i.Year == DateTime.Today.Year.ToString());
            return result.Result.GroupBy((i) => i.Name)
                .Select((t) => new TopItem { Name = t.Key, Score = t.Sum((v) => v.Score), Loved = t.Sum((l) => l.Loved) })
                .OrderByDescending((t) => t.Score);
        }
    }
}
