using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemsWithPartitionKeyToOverallEntries : IMap<IEnumerable<TopItemWithPartitionKey>, IEnumerable<OverallEntry>>
    {
        private readonly IMap<TopItemWithPartitionKey, OverallEntry> mapTopItemWithPartitionKeyToOverallEntry;

        public MapTopItemsWithPartitionKeyToOverallEntries(IMap<TopItemWithPartitionKey,OverallEntry> mapTopItemToOverallEntry)
        {
            this.mapTopItemWithPartitionKeyToOverallEntry = mapTopItemToOverallEntry;
        }

        public IEnumerable<OverallEntry> Map(IEnumerable<TopItemWithPartitionKey> fromItem)
        {
            foreach(var topItem in fromItem)
            {
                yield return mapTopItemWithPartitionKeyToOverallEntry.Map(topItem);
            }
        }
    }
}
