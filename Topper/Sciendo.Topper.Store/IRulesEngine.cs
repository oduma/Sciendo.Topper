using System.Collections.Generic;
using Sciendo.Topper.Domain;

namespace Sciendo.Topper.Store
{
    public interface IRulesEngine
    {
        void AddRule(RuleBase rule);
        void AddRules(IEnumerable<RuleBase> rules);
        void ApplyAllRules(TopItem item);
    }
}