using Microsoft.Extensions.Logging;
using Sciendo.Topper.Domain;

namespace Sciendo.Topper.Store
{
    public class LovedRule:RuleBase
    {
        private readonly ILogger<LovedRule> logger;
        private readonly int _lovedBonus;

        public override void ApplyRule(TopItem item)
        {
            if (item == null)
                return;
            item.Score += (item.Loved * _lovedBonus);
            logger.LogInformation("After calculating score based on rule{2} item {0} has score {1}", item.Name, item.Score, this);
        }

        public LovedRule(ILogger<LovedRule> logger, IRepository<TopItem> topItemsRepo,int lovedBonus) : base(topItemsRepo)
        {
            this.logger = logger;
            _lovedBonus = lovedBonus;
        }
    }
}
