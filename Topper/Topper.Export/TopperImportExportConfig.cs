using Sciendo.Config;
using Sciendo.Topper.Store;

namespace Topper.ImportExport
{
    public class TopperImportExportConfig
    {
        [ConfigProperty("cosmosDb")]
        public CosmosDbConfig CosmosDbConfig { get; set; }

        [ConfigProperty("Csv")]
        public string Csv { get; set; }

        [ConfigProperty("Collection")]
        public string Collection { get; set; }

        [ConfigProperty("Operation")]
        public OperationType OperationType { get; set; }

        [ConfigProperty("ImportTransformations")]
        public ImportTransformation[] ImportTransformations { get; set; }

    }
}
