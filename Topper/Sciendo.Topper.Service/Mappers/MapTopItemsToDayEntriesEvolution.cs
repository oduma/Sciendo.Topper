using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemsToDayEntriesEvolution : IMapAggregateFourEntries<IEnumerable<TopItemWithPictureUrl>, IEnumerable<TopItem>, IEnumerable<DayEntryEvolution>>
    {
        private readonly IMapAggregateFourEntries<TopItemWithPictureUrl, TopItem, DayEntryEvolution> mapTopItemToDayEntryEvolution;

        public MapTopItemsToDayEntriesEvolution(IMapAggregateFourEntries<TopItemWithPictureUrl,TopItem, DayEntryEvolution> mapTopItemToDayEntryEvolution)
        {
            this.mapTopItemToDayEntryEvolution = mapTopItemToDayEntryEvolution;
        }
        public IEnumerable<DayEntryEvolution> Map(IEnumerable<TopItemWithPictureUrl> currentDailyItem, IEnumerable<TopItem> currentOverallItem, IEnumerable<TopItem> previousDailyItem, IEnumerable<TopItem> previouslyOverallItem)
        {
            if (currentDailyItem == null)
                throw new ArgumentNullException(nameof(currentDailyItem));
            foreach(var currentDailyTopItem in currentDailyItem)
            {
                yield return mapTopItemToDayEntryEvolution.Map(currentDailyTopItem, currentOverallItem.FirstOrDefault(o => o.Name == currentDailyTopItem.Name), previousDailyItem.FirstOrDefault(d => d.Name == currentDailyTopItem.Name), previouslyOverallItem.FirstOrDefault(po => po.Name == currentDailyTopItem.Name));
            }
        }
    }
}
