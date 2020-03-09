using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemsWithPartitionKeyToOverallEntriesEvolution : IMapAggregateTwoEntries<IEnumerable<TopItemWithPartitionKey>, IEnumerable<TopItemWithPartitionKey>, IEnumerable<OverallEntryEvolution>>
    {
        private readonly IMapAggregateTwoEntries<TopItemWithPartitionKey, TopItemWithPartitionKey, OverallEntryEvolution> mapTopItemWithPartitionKeyToOverallEntryEvolution;

        public MapTopItemsWithPartitionKeyToOverallEntriesEvolution(IMapAggregateTwoEntries<TopItemWithPartitionKey,TopItemWithPartitionKey, OverallEntryEvolution> mapTopWithPartitionKeyItemToOverallEntryEvolution)
        {
            this.mapTopItemWithPartitionKeyToOverallEntryEvolution = mapTopWithPartitionKeyItemToOverallEntryEvolution;
        }
        public IEnumerable<OverallEntryEvolution> Map(IEnumerable<TopItemWithPartitionKey> currentItem, IEnumerable<TopItemWithPartitionKey> previousItem)
        {
            if (currentItem == null)
                throw new ArgumentNullException(nameof(currentItem));
            if (mapTopItemWithPartitionKeyToOverallEntryEvolution == null)
                throw new Exception("Mapper for overall entry was not provide");
            foreach(var currentTopItem in currentItem)
            {
                yield return mapTopItemWithPartitionKeyToOverallEntryEvolution.Map(currentTopItem, previousItem.FirstOrDefault(p => p.Id == currentTopItem.Id));
            }
        }
    }
}
