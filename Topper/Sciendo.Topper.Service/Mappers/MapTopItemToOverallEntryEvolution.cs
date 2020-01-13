using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemToOverallEntryEvolution : IMapAggregateTwoEntries<TopItemWithPictureUrl, TopItem, OverallEntryEvolution>
    {
        private readonly IMap<TopItem, Position> mapToPosition;

        public MapTopItemToOverallEntryEvolution(IMap<TopItem,Position> mapToPosition)
        {
            this.mapToPosition = mapToPosition;
        }
        public OverallEntryEvolution Map(TopItemWithPictureUrl currentItem, TopItem previousItem)
        {
            if (currentItem == null)
                throw new ArgumentNullException(nameof(currentItem), "current item cannot be null");
            if (previousItem == null)
                return new OverallEntryEvolution(null,mapToPosition.Map(currentItem),currentItem.Name,currentItem.PictureUrl);
            else if (!string.IsNullOrEmpty(currentItem.Name) && !string.IsNullOrEmpty(previousItem.Name) && currentItem.Name != previousItem.Name)
                throw new Exception("Evolution between different items not supported.");
            else
                return new OverallEntryEvolution(mapToPosition.Map(previousItem), mapToPosition.Map(currentItem), currentItem.Name, currentItem.PictureUrl);
        }
    }
}
