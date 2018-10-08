using Newtonsoft.Json;

namespace Sciendo.Topper.Source
{
    public class TopArtistAdditionalAttribute
    {
        [JsonProperty("rank")]
        public int Rank { get; set; }
    }
}
