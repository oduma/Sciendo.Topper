using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Domain.Entities
{
    public class TopItemWithPartitionKey:TopItemBase
    {

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
