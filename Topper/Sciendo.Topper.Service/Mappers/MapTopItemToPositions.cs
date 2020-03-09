using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{

    public class Positions
    {
        public Position DailyPosition { get; set; }

        public Position OverallPosition { get; set; }
    }
    public class MapTopItemToPositions : IMap<TopItem, Positions>
    {

        public Positions Map(TopItem fromItem)
        {
            if (fromItem == null)
                throw new ArgumentNullException(nameof(fromItem));
            return new Positions
            {
                DailyPosition = new Position
                {
                    Rank = fromItem.DayRanking,
                    Hits = fromItem.Hits,
                    NoOfLovedTracks =
                fromItem.Loved,
                    Score = fromItem.Score
                },
                OverallPosition = new Position
                {
                    Rank = fromItem.OverallDayRanking,
                    Hits = fromItem.OverallHits,
                    NoOfLovedTracks =
                fromItem.OverallLoved,
                    Score = fromItem.OverallScore
                }
            };
        }
    }
}
