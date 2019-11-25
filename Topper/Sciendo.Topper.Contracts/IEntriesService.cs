using Sciendo.Topper.Contracts.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Contracts
{
    public interface IEntriesService
    {
        DatedEntry[] GetEntriesByIds(string[] ids);

        DayEntryEvolution[] GetEntriesByDate(DateTime date);

        OverallEntry[] GetEntriesByYear(int year);

        OverallEntryEvolution[] GetEntriesWithEvolutionByYear(int year);
    }
}
