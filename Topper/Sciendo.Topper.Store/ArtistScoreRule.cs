using System.Linq;
using Sciendo.Topper.Contracts;
using Serilog;

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
            Log.Information("{0} potential existing matches found.",potentialMatches?.Count);
            var potentialMatch = potentialMatches
                .FirstOrDefault(p => p.Date.ToString("YYYY-MM-DD") == item.Date.AddDays(-1).ToString("YYYY-MM-DD"));
            if (potentialMatch == null || potentialMatch.Hits < item.Hits)
            {
                Log.Information("Apply bonus. Reason: {0}{1}{2}",
                    (potentialMatch == null) ? "Not found item with name " : "Found the item with name: ", item.Name,
                    (potentialMatch == null)
                        ? "."
                        : $" but with Hits in db = {potentialMatch.Hits} compared with new hits = {item.Hits}");
                AddHitsAndRankingBonus(item);
            }
            else
            {
                Log.Information(
                    "No bonus just hits. Reason: Item in the db with name:{0} with existing hits {1} matched against the new item {2} with new hits {3}",
                    potentialMatch.Name, potentialMatch.Hits, item.Name, item.Hits);
                AddHits(item);
            }
        }

        private void AddHits(TopItem item)
        {
            item.Score += item.Hits;
            Log.Information("After calculating score based on rule{2} item {0} has score {1}", item.Name, item.Score, this);
        }

        private void AddHitsAndRankingBonus(TopItem item)
        {
            item.Score += ((item.Hits == 0) ? 0 : (item.Hits + _rankingBonus/ item.DayRanking));
            Log.Information("After calculating score based on rule{2} item {0} has score {1}", item.Name, item.Score, this);
        }

        public ArtistScoreRule(Repository<TopItem> topItemsRepo,int rankingBonus) : base(topItemsRepo)
        {
            _rankingBonus = rankingBonus;
        }
    }
}
