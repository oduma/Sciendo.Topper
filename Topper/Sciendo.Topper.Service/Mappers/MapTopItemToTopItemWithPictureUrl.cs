using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemToTopItemWithPictureUrl : IMap<TopItem, TopItemWithPictureUrl>
    {
        private readonly IEntryArtistImageProvider entryArtistImageProvider;

        public MapTopItemToTopItemWithPictureUrl(IEntryArtistImageProvider entryArtistImageProvider)
        {
            this.entryArtistImageProvider = entryArtistImageProvider;
        }
        public TopItemWithPictureUrl Map(TopItem fromItem)
        {
            if (fromItem == null)
                throw new ArgumentNullException(nameof(fromItem));

            return new TopItemWithPictureUrl {
                Date = fromItem.Date,
                DayRanking = fromItem.DayRanking,
                Hits = fromItem.Hits,
                Loved=fromItem.Loved,
                Name=fromItem.Name,
                PictureUrl=entryArtistImageProvider.GetPictureUrl(fromItem.Name),
                Score=fromItem.Score,
                Year=fromItem.Year
            };
        }
    }
}
