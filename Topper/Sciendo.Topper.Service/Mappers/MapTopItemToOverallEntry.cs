using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemToOverallEntry : IMap<TopItemWithPictureUrl, OverallEntry>
    {
        private readonly IMap<TopItem, Position> mapTopItemToPosition;

        public MapTopItemToOverallEntry(IMap<TopItem, Position> mapTopItemToPosition)
        {
            this.mapTopItemToPosition = mapTopItemToPosition;
        }
        public OverallEntry Map(TopItemWithPictureUrl fromItem)
        {
            if (mapTopItemToPosition == null)
                throw new Exception("No position mapper");
            if (fromItem == null)
                throw new ArgumentNullException(nameof(fromItem));
            return new OverallEntry(mapTopItemToPosition.Map(fromItem), fromItem.Name, fromItem.PictureUrl);
        }
    }
}
