using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemToEntryTimeLine : IMap<IEnumerable<TopItemWithPictureUrl>, EntryTimeLine>
    {
        private readonly IMap<TopItem, Position> mapToPosition;

        public MapTopItemToEntryTimeLine(IMap<TopItem,Position> mapToPosition)
        {
            this.mapToPosition = mapToPosition;
        }
        public new EntryTimeLine Map(IEnumerable<TopItemWithPictureUrl> fromItem)
        {
            if (fromItem == null)
                throw new ArgumentNullException(nameof(fromItem));
            if (mapToPosition == null)
                throw new Exception("No mapper to position defined.");
            return fromItem.GroupBy(t => t.Name).Select(g => new EntryTimeLine(g.Select(p => new PositionAtDate
            {
                Date = p.Date.ToString("yyyy-MM-dd"),
                Position = mapToPosition.Map(p)
            }).ToArray(), g.Key, g.First().PictureUrl)).FirstOrDefault();
        }
    }
}
