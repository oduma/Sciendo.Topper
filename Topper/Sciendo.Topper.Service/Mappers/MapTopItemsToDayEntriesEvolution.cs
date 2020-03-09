using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemsToDayEntriesEvolution : IMapAggregateTwoEntries<IEnumerable<TopItem>, IEnumerable<TopItem>, IEnumerable<DayEntryEvolution>>
    {
        private readonly IMapAggregateTwoEntries<TopItem, TopItem, DayEntryEvolution> mapTopItemToDayEntryEvolution;

        public MapTopItemsToDayEntriesEvolution(IMapAggregateTwoEntries<TopItem,TopItem, DayEntryEvolution> mapTopItemToDayEntryEvolution)
        {
            this.mapTopItemToDayEntryEvolution = mapTopItemToDayEntryEvolution;
        }
        public IEnumerable<DayEntryEvolution> Map(IEnumerable<TopItem> currentDailyItem,IEnumerable<TopItem> previousDailyItem)
        {
            if (currentDailyItem == null)
                throw new ArgumentNullException(nameof(currentDailyItem));
            foreach(var currentDailyTopItem in currentDailyItem)
            {
                yield return mapTopItemToDayEntryEvolution.Map(currentDailyTopItem, previousDailyItem.FirstOrDefault(d => d.Name == currentDailyTopItem.Name));
            }
        }
    }
}
