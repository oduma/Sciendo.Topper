using Sciendo.Topper.Contracts;

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
        }

        public LovedRule(Repository<TopItem> topItemsRepo,int lovedBonus) : base(topItemsRepo)
        {
            _lovedBonus = lovedBonus;
        }
    }
}
