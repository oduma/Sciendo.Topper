using System;
using Newtonsoft.Json;
using Sciendo.Topper.Domain.Entities;

namespace Sciendo.Topper.Domain
{
    public class TopItem:TopItemBase
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("artist")]
        public string Name { get; set; }

        [JsonProperty("day")]
        public DateTime Date { get; set; }

        [JsonProperty("overallHits")]
        public int OverallHits { get; set; }

        [JsonProperty("noOfOverallLovedTracks")]
        public int OverallLoved { get; set; }

        [JsonProperty("overalScore")]
        public int OverallScore { get; set; }

        [JsonProperty("overallDayRanking")]
        public int OverallDayRanking { get; set; }

    }
}
