using Microsoft.Extensions.Logging;
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
        private readonly ILogger<EntriesService> logger;
        private readonly IRepository<TopItem> repository;
        private readonly IMap<IEnumerable<TopItem>, IEnumerable<EntryTimeLine>> mapTopItemsToEntryTimeLines;
        private readonly IMap<IEnumerable<TopItem>, IEnumerable<OverallEntry>> mapTopItemsToOverallEntries;
        private readonly IMapAggregateTwoEntries<IEnumerable<TopItem>, IEnumerable<OverallEntryEvolution>> mapTopItemsToOverallEntriesEvolution;
        private readonly IMapAggregateFourEntries<IEnumerable<TopItem>, IEnumerable<DayEntryEvolution>> mapTopItemsToDayEntriesEvolution;

        public EntriesService(ILogger<EntriesService> logger,IRepository<TopItem> repository,
            IMap<IEnumerable<TopItem>,IEnumerable<EntryTimeLine>> mapTopItemsToEntryTimeLines,
            IMap<IEnumerable<TopItem>,IEnumerable<OverallEntry>> mapTopItemsToOverallEntries,
            IMapAggregateTwoEntries<IEnumerable<TopItem>,IEnumerable<OverallEntryEvolution>> mapTopItemsToOverallEntriesEvolution,
            IMapAggregateFourEntries<IEnumerable<TopItem>,IEnumerable<DayEntryEvolution>> mapTopItemsToDayEntriesEvolution)
        {
            this.logger = logger;
            this.repository = repository;
            this.mapTopItemsToEntryTimeLines = mapTopItemsToEntryTimeLines;
            this.mapTopItemsToOverallEntries = mapTopItemsToOverallEntries;
            this.mapTopItemsToOverallEntriesEvolution = mapTopItemsToOverallEntriesEvolution;
            this.mapTopItemsToDayEntriesEvolution = mapTopItemsToDayEntriesEvolution;
        }
        public DayEntryEvolution[] GetEntriesByDate(DateTime date)
        {
            logger.LogInformation("Starting Get Entries by Date...");
            try
            {
                return mapTopItemsToDayEntriesEvolution.Map(repository.GetItemsAsync((i) => i.Date == date).Result,
                    AggregateTopItems(GetTopItemsByYear(date.Year, date)),
                    repository.GetItemsAsync(i => i.Date == date.AddDays(-1)).Result,
                    AggregateTopItems(GetTopItemsByYear(date.Year, date.AddDays(-1)))).OrderBy(s=>s.CurrentDayPosition.Rank).ToArray();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "");
                throw ex;
            }
        }

        public EntryTimeLine[] GetEntriesTimeLines(string[] names)
        {
            logger.LogInformation("Started Get Entries by Names");
            try
            {
                return mapTopItemsToEntryTimeLines.Map(repository.GetItemsAsync((t) => names.Contains(t.Name)).Result).ToArray();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "");
                throw ex;
            }
        }

        public OverallEntry[] GetEntriesByYear(int year)
        {
            logger.LogInformation("Started Get Entries by Year.");
            try
            {
                return mapTopItemsToOverallEntries.Map(AggregateTopItems(GetTopItemsByYear(year, DateTime.Now))).ToArray();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "");
                throw ex;
            }
        }

        public OverallEntryEvolution[] GetEntriesWithEvolutionByYear(int year)
        {
            logger.LogInformation("Started Get Entries with Evolution by Year.");
            try
            {
                return mapTopItemsToOverallEntriesEvolution.Map(AggregateTopItems(GetTopItemsByYear(year, DateTime.Now)),
                    AggregateTopItems(GetTopItemsByYear(year, DateTime.Now.AddDays(-1)))).ToArray();

            }
            catch(Exception ex)
            {
                logger.LogError(ex, "");
                throw ex;
            }
        }

        private IEnumerable<TopItem> AggregateTopItems(IEnumerable<TopItem> rawTopItems)
        {
            var topItems = rawTopItems.GroupBy((i) => i.Name)
    .Select((t) => new TopItem { Name = t.Key, Score = t.Sum((v) => v.Score), Loved = t.Sum((l) => l.Loved) })
    .OrderByDescending((t) => t.Score);
            int index = 1;
            foreach (var topItem in topItems)
            {
                topItem.DayRanking = index++;
                yield return topItem;
            }


        }
        private IEnumerable<TopItem> GetTopItemsByYear(int year, DateTime toDate)
        {
            try
            {
                return repository.GetItemsAsync((i) => i.Year == year.ToString() && i.Date <= toDate).Result;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "");
                throw ex;
            }
        }
    }
}
