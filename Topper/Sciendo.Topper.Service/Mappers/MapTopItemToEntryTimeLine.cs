using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemToEntryTimeLine : IMap<IEnumerable<TopItem>, EntryTimeLine>
    {
        private readonly IEntryArtistImageProvider entryArtistImageProvider;

        public MapTopItemToEntryTimeLine(IEntryArtistImageProvider entryArtistImageProvider)
        {
            this.entryArtistImageProvider = entryArtistImageProvider;
        }
        public new EntryTimeLine Map(IEnumerable<TopItem> fromItem)
        {
            if (fromItem == null)
                throw new ArgumentNullException(nameof(fromItem));
            if (entryArtistImageProvider == null)
                throw new Exception("Entry Artist Image Provider is null.");
            return fromItem.GroupBy(t => t.Name).Select(g => new EntryTimeLine { Name = g.Key, 
                PictureUrl= entryArtistImageProvider.GetPictureUrl(g.Key), PositionAtDates = g.Select(p => new PostionAtDate { Date = p.Date.ToString("yyyy-MM-dd"), Position = new Position { Hits = p.Hits, NoOfLovedTracks = p.Loved, Rank = p.DayRanking, Score = p.Score } }).ToArray() }).FirstOrDefault();
        }
    }
}
