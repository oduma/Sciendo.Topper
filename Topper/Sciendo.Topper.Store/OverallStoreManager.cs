using Microsoft.Extensions.Logging;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sciendo.Topper.Store
{
    public class OverallStoreManager : IOverallStoreManager
    {
        private ILogger<OverallStoreManager> logger;
        private IRepository<TopItemWithPartitionKey> repository;
        private readonly IMapper<TopItem, TopItemWithPartitionKey> mapTopItemToTopItemWithPartitionKey;

        public OverallStoreManager(
            ILogger<OverallStoreManager> logger, 
            IRepository<TopItemWithPartitionKey> repository,
            IMapper<TopItem,TopItemWithPartitionKey> mapTopItemToTopItemWithPartitionKey)
        {
            this.logger = logger;
            this.repository = repository;
            this.mapTopItemToTopItemWithPartitionKey = mapTopItemToTopItemWithPartitionKey;
        }

        public TopItemWithPartitionKey[] GetAggregateHistoryOfScores(string date)
        {
            return repository.GetItemsAsync((i) => i.Key == date).Result.ToArray();
        }

        public string[] GetDatesForYear(int year)
        {
            return repository.GetItemsAsync(d => d.Key.StartsWith(year.ToString()))
                .Result
                .Select(k => k.Key)
                .Distinct()
                .OrderByDescending(r => r).ToArray();


        }

        private void StoreItem(TopItemWithPartitionKey topItemWithPartitionKey)
        {
            repository.UpsertItemAsync(topItemWithPartitionKey, topItemWithPartitionKey.Key).Wait();
            logger.LogInformation("Persisted item {0}", topItemWithPartitionKey.Id);
        }
        private TopItemWithPartitionKey[] RankItems(TopItemWithPartitionKey[] topItems)
        {
            int index = 0;
            var previousItemScore = 0;
            foreach (var topItem in topItems)
            {
                if (topItem.Score != previousItemScore)
                    index++;
                topItem.DayRanking = index;
                previousItemScore = topItem.Score;
            }
            return topItems;
        }

        private TopItemWithPartitionKey[] CalculateTopItems(DateTime dateTime, TopItem[] lastDayChanges, string previousDay)
        {
            if(previousDay!=null)
            {
                var dayOverallScores = repository
                    .GetItemsAsync((i) => i.Key == previousDay).Result.Select(t =>
                    {
                        t.Key = dateTime.ToString("yyyy-MM-dd");
                        var lastDayChange = lastDayChanges.FirstOrDefault((i) => i.Name.MakeSuitableForId() == t.Id);
                        if (lastDayChange != null)
                        {
                            t.Loved += lastDayChange.Loved;
                            t.Hits += lastDayChange.Hits;
                            t.Score += lastDayChange.Score;
                            t.DayRanking = lastDayChange.DayRanking;
                        }
                        return t;
                    })
                    .ToArray()
                    .AddNewTopItems(lastDayChanges, mapTopItemToTopItemWithPartitionKey);
                return RankItems(dayOverallScores);
            }
            else
            {
                return RankItems(lastDayChanges.Select(d=>mapTopItemToTopItemWithPartitionKey.Map(d)).OrderByDescending(i=>i.Score).ToArray());
            }
        }

        private string GetPreviousDay(DateTime dateTime)
        {
            var daysRecorded = repository.GetAllItemsAsync()
                .Result
                .Select(i => i.Key)
                .Distinct()
                .OrderByDescending(d=>d);
            if (!daysRecorded.Any())
                return null;
            var proposedLastDay = daysRecorded.FirstOrDefault((d) => Convert.ToDateTime(d) < dateTime);
            if (proposedLastDay == null)
                return null;
            if (Convert.ToInt32(proposedLastDay.Split(new char[] { '-' })[0]) == dateTime.Year)
                return proposedLastDay;
            return null;
        }

        public TopItem[] AdvanceOverallItems(DateTime dateTime, TopItem[] lastDayChanges, out int totalRecordsAfterTheDay)
        {
            DeletePreviousItemsForTheDay(dateTime);
            var previousDay = GetPreviousDay(dateTime);
            var storedItems = UpdateItems(CalculateTopItems(dateTime, lastDayChanges, previousDay));
            totalRecordsAfterTheDay = storedItems.Length;
            logger.LogInformation("For {0} wrote {1} items.",dateTime,storedItems.Length);
            var changesUpdate = lastDayChanges.Select((t) =>
            {
                var dailyItemWithOverallScore = storedItems.FirstOrDefault(i => i.Id == t.Name.MakeSuitableForId());
                if (dailyItemWithOverallScore != null)
                {
                    t.OverallDayRanking = dailyItemWithOverallScore.DayRanking;
                    t.OverallHits = dailyItemWithOverallScore.Hits;
                    t.OverallLoved = dailyItemWithOverallScore.Loved;
                    t.OverallScore = dailyItemWithOverallScore.Score;
                }
                return t;
            }).ToArray();
            if(previousDay!=null)
            {
                var twoDaysAgo = GetPreviousDay(Convert.ToDateTime(previousDay));
                if (twoDaysAgo != null)
                {
                    var oldItems = repository.GetItemsAsync(i => i.Key == twoDaysAgo).Result.ToArray();
                    DeleteItems(oldItems);
                }
            }
            return changesUpdate;
        }

        private void DeleteItems(TopItemWithPartitionKey[] oldItems)
        {
            foreach (var item in oldItems)
            {
                repository.DeleteItem(item.Id, item.Key);
            }
        }

        private void DeletePreviousItemsForTheDay(DateTime dateTime)
        {
            DeleteItems(repository.GetItemsAsync((i) => i.Key == dateTime.ToString("yyyy-MM-dd")).Result.ToArray());
        }

        private TopItemWithPartitionKey[] UpdateItems(TopItemWithPartitionKey[] topItems)
        {
            foreach (var topItem in topItems)
            {
                StoreItem(topItem);
            }
            return topItems;
        }

        public DateTime? NextStartingDate()
        {
            var maxDate = repository.GetAllItemsAsync().Result.Select(t => t.Key).Distinct().Max();
            if (maxDate == null)
                return null;
            var maxRealDate =maxDate.ToDate();
            return maxRealDate.AddDays(1);
        }

        public IEnumerable<TopItemWithPartitionKey> GetAggregateScoresForYear(int year)
        {
            var lastDatePerYear = repository.GetItemsAsync(d=>d.Key.StartsWith(year.ToString())).Result.FirstOrDefault()?.Key;
            return repository.GetItemsAsync((i) => i.Key == lastDatePerYear).Result;
        }
    }
}
