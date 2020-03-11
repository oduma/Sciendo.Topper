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
                var itemsForDate = storeManager.GetItemsByDate(date).ToArray();
                var itemsForYesterday = storeManager.GetLatestPreviousVersionOfItems(date, itemsForDate).ToArray();

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
                var lastDaysPerYear = overallStoreManager.GetDatesForYear(year);
                if (lastDaysPerYear.Length > 2)
                    throw new Exception("Collection for TopItemWithPartitionKey incorrect.");
                if(lastDaysPerYear.Length==2)
                {
                    var lastDay = overallStoreManager.GetAggregateHistoryOfScores(lastDaysPerYear[0]);
                    var secondLastDay = overallStoreManager.GetAggregateHistoryOfScores(lastDaysPerYear[1]);
                    return mapTopItemsWithPartitionKeyToOverallEntriesEvolution.Map(
                        lastDay,
                        secondLastDay).ToArray();
                }
                if (lastDaysPerYear.Length==1)
                {
                    return mapTopItemsWithPartitionKeyToOverallEntriesEvolution.Map(
                        overallStoreManager.GetAggregateHistoryOfScores(lastDaysPerYear[0]),
                        null).ToArray();
                }
                return null;

            }
            catch (Exception ex)
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
