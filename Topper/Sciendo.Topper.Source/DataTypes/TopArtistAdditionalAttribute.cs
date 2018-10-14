using Newtonsoft.Json;

namespace Sciendo.Topper.Source.DataTypes
{
    public class TopArtistAdditionalAttribute
    {
        [JsonProperty("rank")]
        public int Rank { get; set; }
    }
}
