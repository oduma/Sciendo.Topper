using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Store
{
    public class StoreLogic
    {
        public event EventHandler<ProgressEventArgs> Progress;

        public List<TopItemWithScore> StoreItems(List<TopItem> topItems, Repository<TopItemWithScore> itemsRepo,
            int rankingBonus, int lovedBonus)
        {
            var result = new List<TopItemWithScore>();
            foreach (var topItem in topItems)
            {
                result.Add(StoreItem(topItem, itemsRepo, rankingBonus, lovedBonus));
            }

            return result;
        }

        public TopItemWithScore StoreItem(TopItem topItem, Repository<TopItemWithScore> itemsRepo, int rankingBonus,
            int lovedBonus)
        {
            topItem.Date = new DateTime(topItem.Date.Year, topItem.Date.Month, topItem.Date.Day);
            Progress?.Invoke(this, new ProgressEventArgs(topItem, Status.Pending));

            var existingItem =
                itemsRepo.GetItemsAsync(i => i.Name == topItem.Name && i.Date == topItem.Date)
                    .Result.FirstOrDefault();
            if (existingItem == null)
            {
                TopItemWithScore topItemWithScore= new TopItemWithScore();
                if(!(topItem is TopItemWithScore))
                {
                    topItemWithScore =
                    RulesEngine.CalculateScoreForTopItem(topItem, itemsRepo, rankingBonus, lovedBonus);
                }
                else
                {
                    topItemWithScore = topItem as TopItemWithScore;
                }
                topItemWithScore.Year = topItemWithScore.Date.Year.ToString();
                itemsRepo.CreateItemAsync(topItemWithScore);
                Progress?.Invoke(this, new ProgressEventArgs(topItem, Status.Created));
                return topItemWithScore;
            }
            else
            {
                Progress?.Invoke(this, new ProgressEventArgs(existingItem, Status.Existing));
                return existingItem;
            }

        }

        public IEnumerable<TopItemWithScore> GetAggregateHistoryOfScores(Repository<TopItemWithScore> itemsRepo,int limitNumber)
        {
            var result= itemsRepo.GetItemsAsync((i) => i.Year == DateTime.Today.Year.ToString());
            return result.Result.GroupBy((i) => i.Name)
                .Select((t) => new TopItemWithScore { Name = t.Key, Score = t.Sum((v) => v.Score),Loved = t.Sum((l)=>l.Loved) })
                .OrderByDescending((t) => t.Score);
        }
    }
}
