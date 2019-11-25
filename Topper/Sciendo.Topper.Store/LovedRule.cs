using Sciendo.Topper.Domain;
using Serilog;

namespace Sciendo.Topper.Store
{
    public class LovedRule:RuleBase
    {
        private readonly int _lovedBonus;

        public override void ApplyRule(TopItem item)
        {
            if (item == null)
                return;
            item.Score += (item.Loved * _lovedBonus);
            Log.Information("After calculating score based on rule{2} item {0} has score {1}", item.Name, item.Score, this);
        }

        public LovedRule(Repository<TopItem> topItemsRepo,int lovedBonus) : base(topItemsRepo)
        {
            _lovedBonus = lovedBonus;
        }
    }
}
