using Sciendo.Config;
using Sciendo.Topper.Store;

namespace Topper.DataMigration
{
    public class TopperDataMigrationConfig
    {
        [ConfigProperty("cosmosDb")]
        public CosmosDbConfig CosmosDbConfig { get; set; }

        [ConfigProperty("InputFile")]
        public string InoutFile { get; set; }


    }
}
