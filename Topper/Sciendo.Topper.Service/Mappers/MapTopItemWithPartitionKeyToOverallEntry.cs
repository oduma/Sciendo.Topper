using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemWithPartitionKeyToOverallEntry : IMap<TopItemWithPartitionKey, OverallEntry>
    {
        private readonly IMap<TopItemWithPartitionKey, Position> mapTopItemWithPartitionKeyToPosition;
        private readonly IEntryArtistImageProvider entryArtistImageProvider;

        public MapTopItemWithPartitionKeyToOverallEntry(IMap<TopItemWithPartitionKey, Position> mapTopItemWithPartitionKeyToPosition, IEntryArtistImageProvider entryArtistImageProvider)
        {
            this.mapTopItemWithPartitionKeyToPosition = mapTopItemWithPartitionKeyToPosition;
            this.entryArtistImageProvider = entryArtistImageProvider;
        }
        public OverallEntry Map(TopItemWithPartitionKey fromItem)
        {
            if (mapTopItemWithPartitionKeyToPosition == null)
                throw new Exception("No position mapper");
            if (entryArtistImageProvider == null)
                throw new Exception("ImageProvider is mandatory");

            if (fromItem == null)
                throw new ArgumentNullException(nameof(fromItem));
            return new OverallEntry(mapTopItemWithPartitionKeyToPosition.Map(fromItem), fromItem.Id, entryArtistImageProvider.GetPictureUrl(fromItem.Id));
        }
    }
}
