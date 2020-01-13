using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemToDayEntryEvolution : IMapAggregateFourEntries<TopItemWithPictureUrl, TopItem, DayEntryEvolution>
    {
        private readonly IMap<TopItem, Position> mapToPosition;

        public MapTopItemToDayEntryEvolution(IMap<TopItem, Position> mapToPosition) 
        {
            this.mapToPosition = mapToPosition;
        }

        public new DayEntryEvolution Map(TopItemWithPictureUrl currentDailyItem, TopItem currentOverallItem, TopItem previousDailyItem, TopItem previouslyOverallItem)
        {
            if (currentDailyItem == null || string.IsNullOrEmpty(currentDailyItem.Name))
                throw new ArgumentNullException(nameof(currentDailyItem));
            if (currentOverallItem == null || string.IsNullOrEmpty(currentOverallItem.Name))
                throw new ArgumentNullException(nameof(currentOverallItem));
            if(currentDailyItem.Name != currentOverallItem.Name)
                throw new Exception("Cross Items Evolution not supported!");
            if (previouslyOverallItem != null 
                && (string.IsNullOrEmpty(previouslyOverallItem.Name) 
                || previouslyOverallItem.Name!=currentDailyItem.Name))
                throw new Exception("Cross Items Evolution not supported!");
            if (previousDailyItem != null 
                && (string.IsNullOrEmpty(previousDailyItem.Name) 
                || previousDailyItem.Name!=currentDailyItem.Name))
                throw new Exception("Cross Items Evolution not supported!");
            var currentDayPosition = mapToPosition.Map(currentDailyItem);
            currentDayPosition.Rank = (currentDayPosition.Rank == 0) ? 9999 : currentDayPosition.Rank;
            return new DayEntryEvolution(currentDayPosition,
                (previousDailyItem!=null)?mapToPosition.Map(previousDailyItem):null,
                currentDailyItem.Date.Date.ToString("yyyy-MM-dd"),
                (previouslyOverallItem!=null)?mapToPosition.Map(previouslyOverallItem):null,
                mapToPosition.Map(currentOverallItem),
                currentDailyItem.Name,
                currentDailyItem.PictureUrl);
        }

    }
}
