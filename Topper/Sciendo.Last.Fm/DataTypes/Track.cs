using Newtonsoft.Json;

namespace Sciendo.Last.Fm.DataTypes
{
    public class Track:Item
    {
        [JsonProperty("artist")]
        public Item Artist { get; set; }

        [JsonProperty("streamable")]
        public StreamableTrack Streamable { get; set; }
    }
}