using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemToPosition : IMap<TopItem, Position>
    {

        public Position Map(TopItem fromItem)
        {
            if (fromItem == null)
                throw new ArgumentNullException(nameof(fromItem));
            return new Position { 
                Rank = fromItem.DayRanking, 
                Hits = fromItem.Hits, 
                NoOfLovedTracks= 
                fromItem.Loved, 
                Score=fromItem.Score };
        }
    }
}
