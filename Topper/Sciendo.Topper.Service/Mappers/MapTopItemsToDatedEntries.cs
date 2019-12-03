using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemsToDatedEntries : IMap<IEnumerable<TopItem>, IEnumerable<DatedEntry>>
    {
        private readonly IMap<TopItem, DatedEntry> mapTopItemToDatedEntry;

        public MapTopItemsToDatedEntries(IMap<TopItem, DatedEntry> mapTopItemToDatedEntry)
        {
            this.mapTopItemToDatedEntry = mapTopItemToDatedEntry;
        }
        public IEnumerable<DatedEntry> Map(IEnumerable<TopItem> fromItem)
        {
            if (fromItem == null)
                throw new ArgumentNullException(nameof(fromItem));
            foreach(var topItem in fromItem)
            {
                yield return mapTopItemToDatedEntry.Map(topItem);
            }
        }
    }
}
