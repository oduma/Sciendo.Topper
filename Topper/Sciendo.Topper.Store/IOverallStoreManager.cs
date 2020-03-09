using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Sciendo.Topper.Store
{
    public interface IOverallStoreManager
    {
        TopItem[] AdvanceOverallItems(DateTime dateTime, 
            TopItem[] lastDayChanges, 
            out int totalRecordsAfterTheDay);

        IEnumerable<TopItemWithPartitionKey> GetAggregateHistoryOfScores(DateTime date);

        IEnumerable<TopItemWithPartitionKey> GetAggregateScoresForYear(int year);

        DateTime? NextStartingDate();
    }
}