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

        public void StoreItem(TopItem topItem, Repository<TopItem> itemsRepo)
        {
            topItem.Date = new DateTime(topItem.Date.Year, topItem.Date.Month, topItem.Date.Day);
            Progress?.Invoke(this, new ProgressEventArgs(topItem, Status.Pending));

            var existingItem =
                itemsRepo.GetItemsAsync(i => i.Name == topItem.Name && i.Date == topItem.Date)
                    .Result.FirstOrDefault();
            if (existingItem == null)
            {
                topItem.Year = topItem.Date.Year.ToString();
                itemsRepo.CreateItemAsync(topItem);
                Progress?.Invoke(this, new ProgressEventArgs(topItem, Status.Created));
            }
            else
            {
                Progress?.Invoke(this, new ProgressEventArgs(existingItem, Status.Existing));
            }
        }

        public IEnumerable<TopItem> GetAggregateHistoryOfScores(Repository<TopItem> itemsRepo,int limitNumber)
        {
            var result= itemsRepo.GetItemsAsync((i) => i.Year == DateTime.Today.Year.ToString());
            return result.Result.GroupBy((i) => i.Name)
                .Select((t) => new TopItem { Name = t.Key, Score = t.Sum((v) => v.Score),Loved = t.Sum((l)=>l.Loved) })
                .OrderByDescending((t) => t.Score);
        }
    }
}
