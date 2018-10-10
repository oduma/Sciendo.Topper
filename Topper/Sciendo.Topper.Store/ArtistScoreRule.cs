﻿using System.Linq;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Store
{
    public class ArtistScoreRule : RuleBase
    {
        private readonly int _rankingBonus;

        public override void ApplyRule(TopItem item)
        {
            var potentialMatch =
                TopItemsRepo.GetItemsAsync(i => i.Name == item.Name && i.Date == item.Date.AddDays(-1)).Result
                    .FirstOrDefault();
            if (potentialMatch == null || potentialMatch.Hits < item.Hits)
                AddHitsAndRankingBonus(item);
            else
                AddHits(item);


        }

        private void AddHits(TopItem item)
        {
            item.Score += item.Hits;
        }

        private void AddHitsAndRankingBonus(TopItem item)
        {
            item.Score += ((item.Hits == 0) ? 0 : (item.Hits + _rankingBonus));
        }

        public ArtistScoreRule(Repository<TopItem> topItemsRepo,int rankingBonus) : base(topItemsRepo)
        {
            _rankingBonus = rankingBonus;
        }
    }
}
