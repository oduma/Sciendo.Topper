using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemWithPartitionKeyToOverallEntryEvolution : IMapAggregateTwoEntries<TopItemWithPartitionKey, TopItemWithPartitionKey, OverallEntryEvolution>
    {
        private readonly IMap<TopItemWithPartitionKey, Position> mapToPosition;
        private readonly IEntryArtistImageProvider entryArtistImageProvider;

        public MapTopItemWithPartitionKeyToOverallEntryEvolution(IMap<TopItemWithPartitionKey,Position> mapToPosition, IEntryArtistImageProvider entryArtistImageProvider)
        {
            this.mapToPosition = mapToPosition;
            this.entryArtistImageProvider = entryArtistImageProvider;
        }
        public OverallEntryEvolution Map(TopItemWithPartitionKey currentItem, TopItemWithPartitionKey previousItem)
        {
            if (mapToPosition == null)
                throw new Exception("MapToPosition is mandatory");
            if (entryArtistImageProvider == null)
                throw new Exception("ImageProvider is mandatory");

            if (currentItem == null)
                throw new ArgumentNullException(nameof(currentItem), "current item cannot be null");
            if (previousItem == null)
                return new OverallEntryEvolution(null,mapToPosition.Map(currentItem),currentItem.Id,
                    entryArtistImageProvider.GetPictureUrl(currentItem.Id));
            else if (!string.IsNullOrEmpty(currentItem.Id) && !string.IsNullOrEmpty(previousItem.Id) && currentItem.Id != previousItem.Id)
                throw new Exception("Evolution between different items not supported.");
            else
                return new OverallEntryEvolution(mapToPosition.Map(previousItem), mapToPosition.Map(currentItem), currentItem.Id,
                    entryArtistImageProvider.GetPictureUrl(currentItem.Id));
        }
    }
}
