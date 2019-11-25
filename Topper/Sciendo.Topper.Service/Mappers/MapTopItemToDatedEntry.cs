using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemToDatedEntry : MapTopItemToBaseEntry, IMap<TopItem, DatedEntry>
    {
        private readonly IMap<TopItem, Position> mapTopItemToPosition;

        public MapTopItemToDatedEntry(IMap<TopItem,Position> mapTopItemToPosition)
        {
            this.mapTopItemToPosition = mapTopItemToPosition;
        }
        public new DatedEntry Map(TopItem fromItem)
        {
            var oneDayEntry = (DatedEntry)base.Map(fromItem);
            oneDayEntry.Date = fromItem.Date;
            oneDayEntry.Position = mapTopItemToPosition.Map(fromItem);
            return oneDayEntry;
        }
    }
}
