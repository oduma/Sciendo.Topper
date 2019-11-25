using Sciendo.Topper.Contracts;
using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Service.Mappers;
using Sciendo.Topper.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sciendo.Topper.Service
{
    public class EntriesService : IEntriesService
    {
        private readonly IRepository<TopItem> repository;
        private readonly IMap<IEnumerable<TopItem>, IEnumerable<DatedEntry>> mapTopItemsToDatedEntries;
        private readonly IMap<IEnumerable<TopItem>, IEnumerable<OverallEntry>> mapTopItemsToOverallEntries;
        private readonly IMapAggregateTwoEntries<IEnumerable<TopItem>, IEnumerable<OverallEntryEvolution>> mapTopItemsToOverallEntriesEvolution;
        private readonly IMapAggregateFourEntries<IEnumerable<TopItem>, IEnumerable<DayEntryEvolution>> mapTopItemsToDayEntriesEvolution;

        public EntriesService(IRepository<TopItem> repository,
            IMap<IEnumerable<TopItem>,IEnumerable<DatedEntry>> mapTopItemsToDatedEntries,
            IMap<IEnumerable<TopItem>,IEnumerable<OverallEntry>> mapTopItemsToOverallEntries,
            IMapAggregateTwoEntries<IEnumerable<TopItem>,IEnumerable<OverallEntryEvolution>> mapTopItemsToOverallEntriesEvolution,
            IMapAggregateFourEntries<IEnumerable<TopItem>,IEnumerable<DayEntryEvolution>> mapTopItemsToDayEntriesEvolution)
        {
            this.repository = repository;
            this.mapTopItemsToDatedEntries = mapTopItemsToDatedEntries;
            this.mapTopItemsToOverallEntries = mapTopItemsToOverallEntries;
            this.mapTopItemsToOverallEntriesEvolution = mapTopItemsToOverallEntriesEvolution;
            this.mapTopItemsToDayEntriesEvolution = mapTopItemsToDayEntriesEvolution;
        }
        public DayEntryEvolution[] GetEntriesByDate(DateTime date)
        {
            return mapTopItemsToDayEntriesEvolution.Map(repository.GetItemsAsync((i) => i.Date == date).Result,
                AggregateTopItems(GetTopItemsByYear(date.Year, date)),
                repository.GetItemsAsync(i => i.Date == date.AddDays(-1)).Result,
                AggregateTopItems(GetTopItemsByYear(date.Year, date.AddDays(-1)))).ToArray();
        }

        public DatedEntry[] GetEntriesByIds(string[] ids)
        {
            return mapTopItemsToDatedEntries.Map(repository.GetItemsAsync((t) => ids.Contains(t.Name)).Result).ToArray();
        }

        public OverallEntry[] GetEntriesByYear(int year)
        {
            return mapTopItemsToOverallEntries.Map(AggregateTopItems(GetTopItemsByYear(year,DateTime.Now))).ToArray();
        }

        public OverallEntryEvolution[] GetEntriesWithEvolutionByYear(int year)
        {
            return mapTopItemsToOverallEntriesEvolution.Map(AggregateTopItems(GetTopItemsByYear(year,DateTime.Now)), 
                AggregateTopItems(GetTopItemsByYear(year,DateTime.Now.AddDays(-1)))).ToArray();
        }

        private IEnumerable<TopItem> AggregateTopItems(IEnumerable<TopItem> rawTopItems)
        {
            var topItems = rawTopItems.GroupBy((i) => i.Name)
    .Select((t) => new TopItem { Name = t.Key, Score = t.Sum((v) => v.Score), Loved = t.Sum((l) => l.Loved) })
    .OrderByDescending((t) => t.Score);
            int index = 1;
            foreach (var topItem in topItems)
            {
                topItem.DayRanking = index;
                yield return topItem;
            }


        }
        private IEnumerable<TopItem> GetTopItemsByYear(int year, DateTime toDate)
        {
            return repository.GetItemsAsync((i) => i.Year == year.ToString() && i.Date<=toDate).Result;
        }
    }
}
