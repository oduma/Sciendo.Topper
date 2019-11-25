using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemsToOverallEntries : IMap<IEnumerable<TopItem>, IEnumerable<OverallEntry>>
    {
        private readonly IMap<TopItem, OverallEntry> mapTopItemToOverallEntry;

        public MapTopItemsToOverallEntries(IMap<TopItem,OverallEntry> mapTopItemToOverallEntry)
        {
            this.mapTopItemToOverallEntry = mapTopItemToOverallEntry;
        }

        public IEnumerable<OverallEntry> Map(IEnumerable<TopItem> fromItem)
        {
            foreach(var topItem in fromItem)
            {
                yield return mapTopItemToOverallEntry.Map(topItem);
            }
        }
    }
}
