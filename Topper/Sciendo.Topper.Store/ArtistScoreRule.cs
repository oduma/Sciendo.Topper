using System.Linq;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Store
{
    public class ArtistScoreRule : RuleBase
    {
        private readonly int _rankingBonus;

        public override void ApplyRule(TopItem item)
        {
            if (item == null)
                return;
            
            var potentialMatches =
                TopItemsRepo.GetItemsAsync(i => i.Name == item.Name).Result.ToList();
            var potentialMatch = potentialMatches
                .FirstOrDefault(p => p.Date.ToString("YYYY-MM-DD") == item.Date.AddDays(-1).ToString("YYYY-MM-DD"));
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
            item.Score += ((item.Hits == 0) ? 0 : (item.Hits + item.DayRanking * _rankingBonus));
        }

        public ArtistScoreRule(Repository<TopItem> topItemsRepo,int rankingBonus) : base(topItemsRepo)
        {
            _rankingBonus = rankingBonus;
        }
    }
}
