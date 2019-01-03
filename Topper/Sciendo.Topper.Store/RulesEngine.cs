using System;
using System.Collections.Generic;
using System.Text;
using Sciendo.Topper.Contracts;
using Serilog;

namespace Sciendo.Topper.Store
{
    public class RulesEngine
    {
        private readonly List<RuleBase> _rules;

        public RulesEngine()
        {
            _rules = new List<RuleBase>();
        }
        public void AddRule(RuleBase rule)
        {
            if (rule != null)
            {
                Log.Information("Added rule {rule}",rule);
                _rules.Add(rule);
            }
        }

        public void AddRules(IEnumerable<RuleBase> rules)
        {
            _rules.AddRange(rules);
        }
        public void ApplyAllRules(TopItem item)
        {
            Log.Information("Applying {0} rules to {1}",_rules.Count,item.Name);
            foreach (var rule in _rules)
                rule.ApplyRule(item);
            Log.Information("All Rules applied.");
        }
    }
}
