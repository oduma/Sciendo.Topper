using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Store
{
    public static class RulesEngine
    {
        public static TopItemWithScore CalculateScoreForTopItem(TopItem topItem, Repository<TopItemWithScore> topItemsRepo, int Bonus)
        {
            var potentialMatch =
                topItemsRepo.GetItemsAsync(i => i.Name == topItem.Name && i.Date == topItem.Date.AddDays(-1)).Result.FirstOrDefault();
            if (potentialMatch== null )
                return new TopItemWithScore
                {
                    Date = topItem.Date,
                    Hits = topItem.Hits,
                    Name = topItem.Name,
                    Score = topItem.Hits + Bonus

                };
            if (topItem.Hits > potentialMatch.Hits)
            {
                return new TopItemWithScore
                {
                    Date = topItem.Date,
                    Hits = topItem.Hits,
                    Name = topItem.Name,
                    Score = topItem.Hits + Bonus

                };
            }
            return new TopItemWithScore
            {
                Date = topItem.Date,
                Hits = topItem.Hits,
                Name = topItem.Name,
                Score = topItem.Hits
            };

        }
    }
}
