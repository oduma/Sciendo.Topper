using Newtonsoft.Json;

namespace Sciendo.Topper.Source.DataTypes
{
    public class TopArtistsRootObject
    {
        [JsonProperty("topartists")]
        public TopArtistsPage TopArtistsPage { get; set; }
    }
}
