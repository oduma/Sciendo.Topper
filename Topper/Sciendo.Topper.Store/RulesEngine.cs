using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Sciendo.Topper.Domain;

namespace Sciendo.Topper.Store
{
    public class RulesEngine : IRulesEngine
    {
        private readonly List<RuleBase> _rules;
        private readonly ILogger<RulesEngine> logger;

        public RulesEngine(ILogger<RulesEngine> logger)
        {
            _rules = new List<RuleBase>();
            this.logger = logger;
        }
        public void AddRule(RuleBase rule)
        {
            if (rule != null)
            {
                logger.LogInformation("Added rule {rule}", rule);
                _rules.Add(rule);
            }
        }

        public void AddRules(IEnumerable<RuleBase> rules)
        {
            _rules.AddRange(rules);
        }
        public void ApplyAllRules(TopItem item)
        {
            logger.LogInformation("Applying {0} rules to {1}", _rules.Count, item.Name);
            foreach (var rule in _rules)
                rule.ApplyRule(item);
            logger.LogInformation("All Rules applied.");
        }
    }
}
