using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Store
{
    public class MapTopItemToTopItemWithPartitionKey : IMapper<TopItem, TopItemWithPartitionKey>
    {
        public TopItemWithPartitionKey Map(TopItem input)
        {
            return new TopItemWithPartitionKey
            {
                DayRanking = input.DayRanking,
                Hits = input.Hits,
                Id = input.Name.MakeSuitableForId(),
                Key = input.Date.ToString("yyyy-MM-dd"),
                Loved = input.Loved,
                Score = input.Score,
                Year = input.Year
            };
        }

        public TopItem ReverseMap(TopItemWithPartitionKey input)
        {
            return new TopItem
            {
                Date = Convert.ToDateTime(input.Key)
                
            };
        }
    }
}
