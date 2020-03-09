using Microsoft.Extensions.Logging;
using Sciendo.Topper.Contracts;
using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using Sciendo.Topper.Service.Mappers;
using Sciendo.Topper.Store;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sciendo.Topper.Service
{
    public class EntriesService : IEntriesService
    {
        private readonly ILogger<EntriesService> logger;
        private readonly IStoreManager storeManager;
        private readonly IOverallStoreManager overallStoreManager;
        private readonly IMap<DateTimeInterval, TimeInterval> mapDateTimeIntervalToTimeInterval;
        private readonly IMap<IEnumerable<TopItem>, IEnumerable<EntryTimeLine>> mapTopItemsToEntryTimeLines;
        private readonly IMap<IEnumerable<TopItemWithPartitionKey>, IEnumerable<OverallEntry>> mapTopItemsWithPartitionKeyToOverallEntries;
        private readonly IMapAggregateTwoEntries<IEnumerable<TopItemWithPartitionKey>, IEnumerable<TopItemWithPartitionKey>, IEnumerable<OverallEntryEvolution>> mapTopItemsWithPartitionKeyToOverallEntriesEvolution;
        private readonly IMapAggregateTwoEntries<IEnumerable<TopItem>, IEnumerable<TopItem>,IEnumerable<DayEntryEvolution>> mapTopItemsToDayEntriesEvolution;

        public EntriesService(ILogger<EntriesService> logger,
            IStoreManager storeManager,
            IOverallStoreManager overallStoreManager,
            IMap<DateTimeInterval,TimeInterval> mapDateTimeIntervalToTimeInterval,
            IMap<IEnumerable<TopItem>,IEnumerable<EntryTimeLine>> mapTopItemsToEntryTimeLines,
            IMap<IEnumerable<TopItemWithPartitionKey>,IEnumerable<OverallEntry>> mapTopItemsWithPartitionKeyToOverallEntries,
            IMapAggregateTwoEntries<IEnumerable<TopItemWithPartitionKey>,IEnumerable<TopItemWithPartitionKey>, IEnumerable<OverallEntryEvolution>> mapTopItemsWithPartitionKeyToOverallEntriesEvolution,
            IMapAggregateTwoEntries<IEnumerable<TopItem>, IEnumerable<TopItem>, IEnumerable<DayEntryEvolution>> mapTopItemsToDayEntriesEvolution)
        {
            this.logger = logger;
            this.storeManager = storeManager;
            this.overallStoreManager = overallStoreManager;
            this.mapDateTimeIntervalToTimeInterval = mapDateTimeIntervalToTimeInterval;
            this.mapTopItemsToEntryTimeLines = mapTopItemsToEntryTimeLines;
            this.mapTopItemsWithPartitionKeyToOverallEntries = mapTopItemsWithPartitionKeyToOverallEntries;
            this.mapTopItemsWithPartitionKeyToOverallEntriesEvolution = mapTopItemsWithPartitionKeyToOverallEntriesEvolution;
            this.mapTopItemsToDayEntriesEvolution = mapTopItemsToDayEntriesEvolution;
        }
        public DayEntryEvolution[] GetEntriesByDate(DateTime date)
        {
            logger.LogInformation("Starting Get Entries by Date...");
            try
            {
                var itemsForDate = storeManager.GetItemsByDate(date);
                IEnumerable<TopItem> itemsForYesterday = storeManager.GetLatestPreviousVersionOfItems(date, itemsForDate);

                return mapTopItemsToDayEntriesEvolution.Map(itemsForDate,
                    itemsForYesterday)
                    .OrderBy(s => s.CurrentDayPosition.Rank).ToArray();

            }
            catch (Exception ex)
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
                return mapTopItemsToEntryTimeLines.Map(storeManager.GetItemsForNames(names)).ToArray();
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
                return mapTopItemsWithPartitionKeyToOverallEntries.Map(overallStoreManager.GetAggregateScoresForYear(year)).ToArray();
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
                return mapTopItemsWithPartitionKeyToOverallEntriesEvolution.Map(
                    overallStoreManager.GetAggregateHistoryOfScores(DateTime.Now),
                    overallStoreManager.GetAggregateHistoryOfScores(DateTime.Now.AddDays(-1))).ToArray();

            }
            catch(Exception ex)
            {
                logger.LogError(ex, "");
                throw ex;
            }
        }

        public TimeInterval GetTimeInterval()
        {
            try
            {
                return mapDateTimeIntervalToTimeInterval.Map(storeManager.GetTimeInterval());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "");
                throw ex;
            }

        }
    }
}
