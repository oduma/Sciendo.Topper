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

        TopItemWithPartitionKey[] GetAggregateHistoryOfScores(string date);

        string[] GetDatesForYear(int year);

        IEnumerable<TopItemWithPartitionKey> GetAggregateScoresForYear(int year);

        DateTime? NextStartingDate();

    }
}