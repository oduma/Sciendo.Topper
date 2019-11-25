using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemsToDayEntriesEvolution : IMapAggregateFourEntries<IEnumerable<TopItem>, IEnumerable<DayEntryEvolution>>
    {
        private readonly IMapAggregateFourEntries<TopItem, DayEntryEvolution> mapTopItemToDayEntryEvolution;

        public MapTopItemsToDayEntriesEvolution(IMapAggregateFourEntries<TopItem,DayEntryEvolution> mapTopItemToDayEntryEvolution)
        {
            this.mapTopItemToDayEntryEvolution = mapTopItemToDayEntryEvolution;
        }
        public IEnumerable<DayEntryEvolution> Map(IEnumerable<TopItem> currentDailyItem, IEnumerable<TopItem> currentOverallItem, IEnumerable<TopItem> previousDailyItem, IEnumerable<TopItem> previouslyOverallItem)
        {
            foreach(var currentDailyTopItem in currentDailyItem)
            {
                yield return mapTopItemToDayEntryEvolution.Map(currentDailyTopItem, currentOverallItem.FirstOrDefault(o => o.Name == currentDailyTopItem.Name), previousDailyItem.FirstOrDefault(d => d.Name == currentDailyTopItem.Name), previouslyOverallItem.FirstOrDefault(po => po.Name == currentDailyTopItem.Name));
            }
        }
    }
}
