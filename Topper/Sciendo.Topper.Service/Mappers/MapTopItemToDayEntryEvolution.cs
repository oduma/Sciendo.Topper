using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using System;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemToDayEntryEvolution : MapTopItemToOverallEntryEvolution, IMapAggregateFourEntries<TopItem, DayEntryEvolution>
    {
        public MapTopItemToDayEntryEvolution(IMap<TopItem, Position> mapToOverallPosition,IEntryArtistImageProvider entryArtistImageProvider) 
            : base(mapToOverallPosition, entryArtistImageProvider)
        {
        }

        public new DayEntryEvolution Map(TopItem currentDailyItem, TopItem currentOverallItem, TopItem previousDailyItem, TopItem previouslyOverallItem)
        {

            var overallEntryEvolution = base.Map(currentOverallItem, previouslyOverallItem);
            var dailyEntryEvolution = base.Map(currentDailyItem, previousDailyItem);
            if (overallEntryEvolution != null
                && currentDailyItem != null
                && !string.IsNullOrEmpty(overallEntryEvolution.Name)
                && !string.IsNullOrEmpty(dailyEntryEvolution.Name)
                && overallEntryEvolution.Name != dailyEntryEvolution.Name)
                throw new Exception("Cross Items Evolution not supported!");

            var dayEntryEvolution = new DayEntryEvolution { 
                Name = dailyEntryEvolution.Name,
                PictureUrl=dailyEntryEvolution.PictureUrl,
                CurrentDayPosition = dailyEntryEvolution.CurrentOverallPosition, 
                PreviousDayPosition = dailyEntryEvolution.PreviousDayOverallPosition, 
                CurrentOverallPosition = overallEntryEvolution.CurrentOverallPosition, 
                PreviousDayOverallPosition = overallEntryEvolution.PreviousDayOverallPosition,
                Date= currentDailyItem.Date.Date.ToString("yyyy-MM-dd")};
            dayEntryEvolution.CurrentDayPosition.Rank = 
                (dayEntryEvolution.CurrentDayPosition.Rank == 0) ? 9999 : dayEntryEvolution.CurrentDayPosition.Rank;

            return dayEntryEvolution;
        }

    }
}
