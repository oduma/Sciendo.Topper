namespace Sciendo.Topper.Store
{
    public class CosmosDbConfig
    {
        public string Endpoint { get; set; }
        public string Key { get; set; }
        public string DatabaseId { get; set; }

        public CosmosDbCollection[] CosmosDbCollections { get; set; }

    }
}