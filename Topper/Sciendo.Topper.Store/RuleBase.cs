using System;
using Sciendo.Topper.Domain;

namespace Sciendo.Topper.Store
{
    public abstract class RuleBase
    {
        protected RuleBase(IRepository<TopItem> topItemsRepo)
        {
            if(topItemsRepo==null)
                throw new ArgumentNullException(nameof(topItemsRepo));
            TopItemsRepo = topItemsRepo;
        }

        protected IRepository<TopItem> TopItemsRepo { get; }

        public abstract void ApplyRule(TopItem item);
    }
}
