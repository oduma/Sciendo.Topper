using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemsToOverallEntries : IMap<IEnumerable<TopItemWithPictureUrl>, IEnumerable<OverallEntry>>
    {
        private readonly IMap<TopItemWithPictureUrl, OverallEntry> mapTopItemToOverallEntry;

        public MapTopItemsToOverallEntries(IMap<TopItemWithPictureUrl,OverallEntry> mapTopItemToOverallEntry)
        {
            this.mapTopItemToOverallEntry = mapTopItemToOverallEntry;
        }

        public IEnumerable<OverallEntry> Map(IEnumerable<TopItemWithPictureUrl> fromItem)
        {
            foreach(var topItem in fromItem)
            {
                yield return mapTopItemToOverallEntry.Map(topItem);
            }
        }
    }
}
