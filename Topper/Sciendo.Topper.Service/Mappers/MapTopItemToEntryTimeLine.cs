using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemToEntryTimeLine : IMap<IEnumerable<TopItem>, EntryTimeLine>
    {
        private readonly IMap<TopItem, Positions> mapToPositions;
        private readonly IEntryArtistImageProvider entryArtistImageProvider;

        public MapTopItemToEntryTimeLine(IMap<TopItem,Positions> mapToPositions, IEntryArtistImageProvider entryArtistImageProvider)
        {
            this.mapToPositions = mapToPositions;
            this.entryArtistImageProvider = entryArtistImageProvider;
        }
        public new EntryTimeLine Map(IEnumerable<TopItem> fromItem)
        {
            if (mapToPositions == null)
                throw new Exception("MapToPositions is mandatory");
            if (entryArtistImageProvider == null)
                throw new Exception("ImageProvider is mandatory");

            if (fromItem == null)
                throw new ArgumentNullException(nameof(fromItem));
            if (mapToPositions == null)
                throw new Exception("No mapper to position defined.");
            return fromItem.GroupBy(t => t.Name).Select(g => new EntryTimeLine(g.Select(p => new PositionAtDate
            {
                Date = p.Date.ToString("yyyy-MM-dd"),
                Position = mapToPositions.Map(p).DailyPosition
            }).ToArray(), g.Key,  entryArtistImageProvider.GetPictureUrl(g.First().Name))).FirstOrDefault();
        }
    }
}
