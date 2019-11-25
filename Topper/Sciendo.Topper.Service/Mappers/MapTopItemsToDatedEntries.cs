using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemsToDatedEntries : IMap<IEnumerable<TopItem>, IEnumerable<DatedEntry>>
    {
        private readonly IMap<TopItem, DatedEntry> mapTopItemToDetailEntry;

        public MapTopItemsToDatedEntries(IMap<TopItem, DatedEntry> mapTopItemToDetailEntry)
        {
            this.mapTopItemToDetailEntry = mapTopItemToDetailEntry;
        }
        public IEnumerable<DatedEntry> Map(IEnumerable<TopItem> fromItem)
        {
            foreach(var topItem in fromItem)
            {
                yield return mapTopItemToDetailEntry.Map(topItem);
            }
        }
    }
}
