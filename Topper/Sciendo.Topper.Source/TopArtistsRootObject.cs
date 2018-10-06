using Newtonsoft.Json;

namespace Sciendo.Topper.Source
{
    public class TopArtistsRootObject
    {
        [JsonProperty("topartists")]
        public TopArtistsPage TopArtistsPage { get; set; }
    }
}
