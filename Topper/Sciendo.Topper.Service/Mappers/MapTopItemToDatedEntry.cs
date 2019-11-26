using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemToDatedEntry : MapTopItemToOverallEntry, IMap<TopItem, DatedEntry>
    {

        public MapTopItemToDatedEntry(IMap<TopItem,Position> mapTopItemToPosition):base(mapTopItemToPosition)
        {
        }
        public new DatedEntry Map(TopItem fromItem)
        {
            if (fromItem == null)
                throw new ArgumentNullException(nameof(fromItem));
            var overallEntry = base.Map(fromItem);
            return new DatedEntry { Name = overallEntry.Name, CurrentOverallPosition = overallEntry.CurrentOverallPosition,Date=fromItem.Date };
        }
    }
}
