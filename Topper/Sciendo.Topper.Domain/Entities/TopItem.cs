using System;
using Newtonsoft.Json;

namespace Sciendo.Topper.Domain
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

        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("year")]
        public string Year { get; set; }

        [JsonProperty("dayRanking")]
        public int DayRanking { get; set; }
    }
}
