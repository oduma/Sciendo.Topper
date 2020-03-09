using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;

namespace Sciendo.Topper.Store
{
    public class StoreManager : IStoreManager
    {

        private readonly ILogger<StoreManager> logger;
        private IRepository<TopItem> _itemsRepo;
        public StoreManager(
            ILogger<StoreManager> logger, 
            IRepository<TopItem> itemsRepo)
        {
            this.logger = logger;
            _itemsRepo = itemsRepo ?? throw new ArgumentNullException(nameof(itemsRepo));
        }

        private void StoreItem(TopItem topItem)
        {
            if (topItem == null || topItem.Date == DateTime.MinValue || string.IsNullOrEmpty(topItem.Name) ||
                (topItem.Hits == 0 && topItem.Loved == 0))
                throw new ArgumentNullException(nameof(topItem));
            _itemsRepo.UpsertItemAsync(topItem).Wait();
            logger.LogInformation("Persisted item {0}", topItem.Name);
        }

        public IEnumerable<TopItem> GetLatestPreviousVersionOfItems(DateTime queryDate, IEnumerable<TopItem> itemsForDate)
        {
            var itemsForYesterday = _itemsRepo.GetItemsAsync(i =>
            i.Date == queryDate.AddDays(-1)).Result.Where(r=>
                itemsForDate.Contains(r,new TopItemEqualityComparer()));

            var itemsMissingBetweenTheDays = itemsForDate.Where((i) => !itemsForYesterday.Contains(i, new TopItemEqualityComparer()));
            if (itemsMissingBetweenTheDays.Any())
            {
                var olderItems = _itemsRepo.GetItemsAsync(i =>
                      i.Date < queryDate && i.Year == queryDate.Year.ToString())
                    .Result
                    .Where(r=> itemsMissingBetweenTheDays.Contains(r,new TopItemEqualityComparer()))
                    .GroupBy(n => n.Name).Select((r) =>
                    {
                        var latest = r.OrderByDescending(g => g.Date).First();
                        return new TopItem
                        {
                            Name = r.Key,
                            Date = queryDate.AddDays(-1),
                            Hits = 0,
                            DayRanking = 0,
                            Loved = 0,
                            OverallDayRanking = latest.OverallDayRanking,
                            OverallHits = latest.OverallHits,
                            OverallLoved = latest.OverallLoved,
                            OverallScore = latest.OverallScore,
                            Score = 0,
                            Year = queryDate.Year.ToString()
                        };
                    });
                if (olderItems.Any())
                    itemsForYesterday = itemsForYesterday.Union(olderItems);
            }

            return itemsForYesterday;
        }

        public void UpdateItems(IEnumerable<TopItem> topItems)
        {
            foreach(var topItem in topItems)
            {
                StoreItem(topItem);
            }
        }

        public IEnumerable<TopItem> GetItemsByDate(DateTime date)
        {
            return _itemsRepo.GetItemsAsync((i) => i.Date == date).Result;
        }

        public IEnumerable<TopItem> GetItemsForNames(IEnumerable<string> names)
        {
            return _itemsRepo.GetAllItemsAsync().Result.Where(r=>names.Contains(r.Name));
        }

        public DateTimeInterval GetTimeInterval()
        {
            var firstItem = _itemsRepo.GetAllItemsAsync().Result.Min(i => i.Date);
            var lastItem = _itemsRepo.GetAllItemsAsync().Result.Max(i => i.Date);
            return new DateTimeInterval
            {
                FromDate = firstItem,
                ToDate = lastItem
            };
        }
    }
}
