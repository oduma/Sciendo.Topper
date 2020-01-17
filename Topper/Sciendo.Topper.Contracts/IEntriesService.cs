using Sciendo.Topper.Contracts.DataTypes;
using System;

namespace Sciendo.Topper.Contracts
{
    public interface IEntriesService
    {
        EntryTimeLine[] GetEntriesTimeLines(string[] names);

        DayEntryEvolution[] GetEntriesByDate(DateTime date);

        OverallEntry[] GetEntriesByYear(int year);

        OverallEntryEvolution[] GetEntriesWithEvolutionByYear(int year);
        TimeInterval GetTimeInterval();
    }
}
