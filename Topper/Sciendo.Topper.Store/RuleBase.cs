using System;
using System.Collections.Generic;
using System.Text;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Store
{
    public abstract class RuleBase
    {
        protected RuleBase(Repository<TopItem> topItemsRepo)
        {
            TopItemsRepo = topItemsRepo;
        }

        protected Repository<TopItem> TopItemsRepo { get; }

        public abstract void ApplyRule(TopItem item);
    }
}
