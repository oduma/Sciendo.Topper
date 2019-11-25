using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using System;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemToOverallEntry : MapTopItemToBaseEntry, IMap<TopItem, OverallEntry>
    {
        private readonly IMap<TopItem, Position> mapTopItemToPosition;

        public MapTopItemToOverallEntry(IMap<TopItem, Position> mapTopItemToPosition)
        {
            this.mapTopItemToPosition = mapTopItemToPosition;
        }
        public new OverallEntry Map(TopItem fromItem)
        {
            if (mapTopItemToPosition == null)
                throw new Exception("No position mapper");
            if (fromItem == null)
                throw new ArgumentNullException(nameof(fromItem));
            OverallEntry overallEntry = new OverallEntry { Name = base.Map(fromItem).Name };
            overallEntry.Position = mapTopItemToPosition.Map(fromItem);
            return overallEntry;
        }
    }
}
