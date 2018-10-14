using System;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Store
{
    public abstract class RuleBase
    {
        protected RuleBase(Repository<TopItem> topItemsRepo)
        {
            if(topItemsRepo==null)
                throw new ArgumentNullException(nameof(topItemsRepo));
            TopItemsRepo = topItemsRepo;
        }

        protected Repository<TopItem> TopItemsRepo { get; }

        public abstract void ApplyRule(TopItem item);
    }
}
