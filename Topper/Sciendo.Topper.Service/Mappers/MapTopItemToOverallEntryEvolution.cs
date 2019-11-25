using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemToOverallEntryEvolution : MapTopItemToOverallEntry, IMapAggregateTwoEntries<TopItem, OverallEntryEvolution>
    {
        public MapTopItemToOverallEntryEvolution(IMap<TopItem,Position> mapToOverallPosition):base(mapToOverallPosition)
        {
        }
        public OverallEntryEvolution Map(TopItem currentItem, TopItem previousItem)
        {
            if (currentItem == null && previousItem == null)
                throw new ArgumentNullException("both topitems cannot be null");
            OverallEntryEvolution overallEntryEvolution;
            if (currentItem == null)
            {
                var overallEntry = base.Map(previousItem);
                overallEntryEvolution = new OverallEntryEvolution { Name = overallEntry.Name, PreviousDayOverallPosition = overallEntry.Position };
                overallEntryEvolution.Position = null;
                return overallEntryEvolution;

            }
            else if (previousItem == null)
            {
                var overallEntry = base.Map(currentItem);
                overallEntryEvolution = new OverallEntryEvolution { Name=overallEntry.Name,Position=overallEntry.Position};
                overallEntryEvolution.PreviousDayOverallPosition = null;
                return overallEntryEvolution;
            }
            else if (!string.IsNullOrEmpty(currentItem.Name) && !string.IsNullOrEmpty(previousItem.Name) && currentItem.Name != previousItem.Name)
            {
                throw new Exception("Evolution between different items not supported.");
            }
            else
            {
                var overallEntry = base.Map(currentItem);
                overallEntryEvolution = new OverallEntryEvolution { Name = overallEntry.Name, Position = overallEntry.Position };
                overallEntryEvolution.PreviousDayOverallPosition = base.Map(previousItem).Position;
                return overallEntryEvolution;
            }
        }
    }
}
