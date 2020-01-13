using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemsToTopItemsWithPictureUrl : IMap<IEnumerable<TopItem>, IEnumerable<TopItemWithPictureUrl>>
    {
        private readonly IMap<TopItem, TopItemWithPictureUrl> mapToTopItemWithPictureUrl;

        public MapTopItemsToTopItemsWithPictureUrl(IMap<TopItem,TopItemWithPictureUrl> mapToTopItemWithPictureUrl)
        {
            this.mapToTopItemWithPictureUrl = mapToTopItemWithPictureUrl;
        }
        public IEnumerable<TopItemWithPictureUrl> Map(IEnumerable<TopItem> fromItem)
        {
            foreach(var item in fromItem)
            {
                yield return mapToTopItemWithPictureUrl.Map(item);
            }
        }
    }
}
