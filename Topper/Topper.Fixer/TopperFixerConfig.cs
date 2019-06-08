using System;
using System.Collections.Generic;
using System.Text;
using Sciendo.Config;
using Sciendo.Topper.Store;

namespace Topper.Fixer
{
    public class TopperFixerConfig
    {
        [ConfigProperty("cosmosDb")]
        public CosmosDbConfig CosmosDbConfig { get; set; }

        [ConfigProperty("startFrom")]
        public DateTime StartFrom { get; set; }

        [ConfigProperty("topperRules")]
        public TopperRulesConfig TopperRulesConfig { get; set; }

    }
}
