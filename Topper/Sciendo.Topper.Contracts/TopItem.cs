using System;
using Newtonsoft.Json;

namespace Sciendo.Topper.Contracts
{
    public class TopItem
    {
        [JsonProperty("artist")]
        public string Name { get; set; }

        [JsonProperty("hits")]
        public int Hits { get; set; }

        [JsonProperty("day")]
        public DateTime Date { get; set; }

        [JsonProperty("noOfLovedTracks")]
        public int Loved { get; set; }
    }
}
