using System;
using System.Collections.Generic;
using System.Text;
using Sciendo.Topper.Contracts;

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
            if(rule!=null)
                _rules.Add(rule);
        }

        public void AddRules(IEnumerable<RuleBase> rules)
        {
            _rules.AddRange(rules);
        }
        public void ApplyAllRules(TopItem item)
        {
            foreach (var rule in _rules)
                rule.ApplyRule(item);
        }
    }
}
