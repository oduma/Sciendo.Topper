using System;
using System.Collections.Generic;
using System.Linq;
using Sciendo.Topper.Contracts;

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
            Progress?.Invoke(this, new ProgressEventArgs(topItem, Status.Pending));
            topItem.Date = new DateTime(topItem.Date.Year, topItem.Date.Month, topItem.Date.Day);

            var existingItem =
                _itemsRepo.GetItemsAsync(i => i.Name == topItem.Name && i.Date == topItem.Date)
                    .Result.FirstOrDefault();
            if (existingItem == null)
            {
                topItem.Year = topItem.Date.Year.ToString();
                _itemsRepo.CreateItemAsync(topItem);
                Progress?.Invoke(this, new ProgressEventArgs(topItem, Status.Created));
            }
            else
            {
                Progress?.Invoke(this, new ProgressEventArgs(existingItem, Status.Existing));
            }
        }

        public IEnumerable<TopItem> GetAggregateHistoryOfScores()
        {
            var result= _itemsRepo.GetItemsAsync((i) => i.Year == DateTime.Today.Year.ToString());
            return result.Result.GroupBy((i) => i.Name)
                .Select((t) => new TopItem { Name = t.Key, Score = t.Sum((v) => v.Score),Loved = t.Sum((l)=>l.Loved) })
                .OrderByDescending((t) => t.Score);
        }
    }
}
