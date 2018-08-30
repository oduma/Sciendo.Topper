using Newtonsoft.Json;

namespace Sciendo.Topper.Source
{
    public class RootObject
    {
        [JsonProperty("topartists")]
        public TopArtistsPage TopArtistsPage { get; set; }
    }
}
