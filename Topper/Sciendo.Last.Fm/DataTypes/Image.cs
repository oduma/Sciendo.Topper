using Newtonsoft.Json;

namespace Sciendo.Last.Fm.DataTypes
{
    public class Image
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("size")]
        public Size Size { get; set; }
    }
}
