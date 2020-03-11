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

        [ConfigProperty("CurrentSourceCollectionId")]
        public string CurrentSourceCollectionId { get; set; }
        [ConfigProperty("CurrentTargetCollectionId")]
        public string CurrentTargetCollectionId { get; set; }
    }
}
