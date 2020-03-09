using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Store
{
    public class CosmosDbCollection
    {
        public string CollectionId { get; set; }

        public string TypeOfItem { get; set; }

        public string PartitionKeyName { get; set; }

    }
}
