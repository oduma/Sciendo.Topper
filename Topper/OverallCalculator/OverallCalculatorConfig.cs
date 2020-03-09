using Sciendo.Config;
using Sciendo.Last.Fm;
using Sciendo.Topper.Store;
using System;
using System.Collections.Generic;
using System.Text;

namespace OverallCalculator
{
    public class OverallCalculatorConfig
    {
        [ConfigProperty("cosmosDb")]
        public CosmosDbConfig CosmosDbConfig { get; set; }

        [ConfigProperty("StartDate")]
        public string StartDate { get; set; }

        [ConfigProperty("CurrentSourceCollection")]
        public string CurrentSourceCollectionId { get; set; }
        [ConfigProperty("CurrentTargetCollection")]
        public string CurrentTargetCollectionId { get; set; }
    }
}
