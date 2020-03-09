using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemsToEntryTimeLines : IMap<IEnumerable<TopItem>, IEnumerable<EntryTimeLine>>
    {
        private readonly IMap<IEnumerable<TopItem>, EntryTimeLine> mapTopItemToEntryTileLine;

        public MapTopItemsToEntryTimeLines(IMap<IEnumerable<TopItem>, EntryTimeLine> mapTopItemToDatedEntry)
        {
            this.mapTopItemToEntryTileLine = mapTopItemToDatedEntry;
        }
        public IEnumerable<EntryTimeLine> Map(IEnumerable<TopItem> fromItem)
        {
            if (fromItem == null)
                throw new ArgumentNullException(nameof(fromItem));
            return fromItem.GroupBy(g => g.Name).Select(g => mapTopItemToEntryTileLine.Map(g));
        }
    }
}
