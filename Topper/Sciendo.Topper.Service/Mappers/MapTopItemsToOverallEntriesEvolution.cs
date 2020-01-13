using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemsToOverallEntriesEvolution : IMapAggregateTwoEntries<IEnumerable<TopItemWithPictureUrl>, IEnumerable<TopItem>, IEnumerable<OverallEntryEvolution>>
    {
        private readonly IMapAggregateTwoEntries<TopItemWithPictureUrl, TopItem, OverallEntryEvolution> mapTopItemToOverallEntryEvolution;

        public MapTopItemsToOverallEntriesEvolution(IMapAggregateTwoEntries<TopItemWithPictureUrl,TopItem, OverallEntryEvolution> mapTopItemToOverallEntryEvolution)
        {
            this.mapTopItemToOverallEntryEvolution = mapTopItemToOverallEntryEvolution;
        }
        public IEnumerable<OverallEntryEvolution> Map(IEnumerable<TopItemWithPictureUrl> currentItem, IEnumerable<TopItem> previousItem)
        {
            if (currentItem == null)
                throw new ArgumentNullException(nameof(currentItem));
            if (mapTopItemToOverallEntryEvolution == null)
                throw new Exception("Mapper for overall entry was not provide");
            foreach(var currentTopItem in currentItem)
            {
                yield return mapTopItemToOverallEntryEvolution.Map(currentTopItem, previousItem.FirstOrDefault(p => p.Name == currentTopItem.Name));
            }
        }
    }
}
