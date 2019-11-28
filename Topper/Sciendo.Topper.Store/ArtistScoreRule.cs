using System.Linq;
using Microsoft.Extensions.Logging;
using Sciendo.Topper.Domain;

namespace Sciendo.Topper.Store
{
    public class ArtistScoreRule : RuleBase
    {
        private readonly ILogger<ArtistScoreRule> logger;
        private readonly int _rankingBonus;

        public override void ApplyRule(TopItem item)
        {
            if (item == null)
                return;
            
            var potentialMatches =
                TopItemsRepo.GetItemsAsync(i => i.Name == item.Name).Result.ToList();
            logger.LogInformation("{0} potential existing matches found.",potentialMatches?.Count);
            logger.LogInformation("Looking for potential matches in {0}",item.Date.AddDays(-1).ToString("yyyy-MM-dd"));
            var potentialMatch = potentialMatches
                .FirstOrDefault(p => p.Date.ToString("yyyy-MM-dd") == item.Date.AddDays(-1).ToString("yyyy-MM-dd") && p.Hits>0);
            if (potentialMatch == null || potentialMatch.Hits < item.Hits)
            {
                logger.LogInformation("Apply bonus. Reason: {0}{1}{2}",
                    (potentialMatch == null) ? "Not found item with name " : "Found the item with name: ", item.Name,
                    (potentialMatch == null)
                        ? "."
                        : $" but with Hits in db = {potentialMatch.Hits} compared with new hits = {item.Hits}");
                AddHitsAndRankingBonus(item);
            }
            else
            {
                logger.LogInformation(
                    "No bonus just hits. Reason: Item in the db with name:{0} with existing hits {1} matched against the new item {2} with new hits {3}",
                    potentialMatch.Name, potentialMatch.Hits, item.Name, item.Hits);
                AddHits(item);
            }
        }

        private void AddHits(TopItem item)
        {
            item.Score += item.Hits;
            logger.LogInformation("After calculating score based on rule{2} item {0} has score {1}", item.Name, item.Score, this);
        }

        private void AddHitsAndRankingBonus(TopItem item)
        {
            item.Score += ((item.Hits == 0) ? 0 : (item.Hits + _rankingBonus/ item.DayRanking));
            logger.LogInformation("After calculating score based on rule{2} item {0} has score {1}", item.Name, item.Score, this);
        }

        public ArtistScoreRule(ILogger<ArtistScoreRule> logger, IRepository<TopItem> topItemsRepo,int rankingBonus) : base(topItemsRepo)
        {
            this.logger = logger;
            _rankingBonus = rankingBonus;
        }
    }
}
