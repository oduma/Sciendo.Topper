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
        public void StoreItems(TopItem[] topItems, Repository<TopItemWithScore> itemsRepo, int bonus)
        {
            foreach (var topItem in topItems)
            {
                topItem.Date = new DateTime(topItem.Date.Year, topItem.Date.Month, topItem.Date.Day);
                if(Progress!=null)
                    Progress(this,new ProgressEventArgs(topItem,Status.Pending));
                var existingItem =
                    itemsRepo.GetItemsAsync(i => i.Name == topItem.Name && i.Date == topItem.Date)
                        .Result.FirstOrDefault();
                if (existingItem == null)
                {
                    var result = itemsRepo.CreateItemAsync(RulesEngine.CalculateScoreForTopItem(topItem, itemsRepo, bonus));
                    if(Progress!=null)
                        Progress(this,new ProgressEventArgs(topItem,Status.Created));
                }
                else
                {
                    if(Progress!=null)
                        Progress(this,new ProgressEventArgs(existingItem,Status.Existing));
                }
            }

        }
    }
}
