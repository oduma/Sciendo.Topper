using Sciendo.Config;
using Sciendo.Topper.Store;

namespace Topper.Export
{
    public class TopperExportConfig
    {
        [ConfigProperty("cosmosDb")]
        public CosmosDbConfig CosmosDbConfig { get; set; }

        [ConfigProperty("OutputFile")]
        public string OutputFile { get; set; }

        [ConfigProperty("Collection")]
        public string Collection
        {
            get => CosmosDbConfig.CollectionId;
            set => CosmosDbConfig.CollectionId = value;
        }

    }
}
