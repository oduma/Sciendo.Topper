using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemsToOverallEntriesEvolution : IMapAggregateTwoEntries<IEnumerable<TopItem>, IEnumerable<OverallEntryEvolution>>
    {
        private readonly IMapAggregateTwoEntries<TopItem, OverallEntryEvolution> mapTopItemToOverallEntryEvolution;

        public MapTopItemsToOverallEntriesEvolution(IMapAggregateTwoEntries<TopItem,OverallEntryEvolution> mapTopItemToOverallEntryEvolution)
        {
            this.mapTopItemToOverallEntryEvolution = mapTopItemToOverallEntryEvolution;
        }
        public IEnumerable<OverallEntryEvolution> Map(IEnumerable<TopItem> currentItem, IEnumerable<TopItem> previousItem)
        {
            if (mapTopItemToOverallEntryEvolution == null)
                throw new Exception("Mapper for overall entry was not provide");
            foreach(var currentTopItem in currentItem)
            {
                yield return mapTopItemToOverallEntryEvolution.Map(currentTopItem, previousItem.FirstOrDefault(p => p.Name == currentTopItem.Name));
            }
        }
    }
}
