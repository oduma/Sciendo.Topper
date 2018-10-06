using System;
using System.Collections.Generic;
using System.Text;
using Sciendo.Config;
using Sciendo.Topper.Store;

namespace Topper.DataMigration
{
    public class TopperDataMigrationConfig
    {
        [ConfigProperty("cosmosDb")]
        public CosmosDb CosmosDb { get; set; }

        [ConfigProperty("topperRules")]
        public  TopperRulesConfig TopperRulesConfig { get; set; }

        [ConfigProperty("InputFile")]
        public string InoutFile { get; set; }


    }
}
