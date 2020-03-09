using Newtonsoft.Json;

namespace Sciendo.Topper.Domain.Entities
{
    public abstract class TopItemBase
    {

        [JsonProperty("year")]
        public string Year { get; set; }

        [JsonProperty("hits")]
        public int Hits { get; set; }

        [JsonProperty("noOfLovedTracks")]
        public int Loved { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("dayRanking")]
        public int DayRanking { get; set; }

    }
}
