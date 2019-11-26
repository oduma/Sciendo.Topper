using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemToDayEntryEvolution : MapTopItemToOverallEntryEvolution, IMapAggregateFourEntries<TopItem, DayEntryEvolution>
    {
        public MapTopItemToDayEntryEvolution(IMap<TopItem, Position> mapToOverallPosition) : base(mapToOverallPosition)
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
                CurrentDayPosition = dailyEntryEvolution.CurrentOverallPosition, 
                PreviousDayPosition = dailyEntryEvolution.PreviousDayOverallPosition, 
                CurrentOverallPosition = overallEntryEvolution.CurrentOverallPosition, 
                PreviousDayOverallPosition = overallEntryEvolution.PreviousDayOverallPosition };

            return dayEntryEvolution;
        }

    }
}
