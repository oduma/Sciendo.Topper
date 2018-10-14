using System.Collections.Generic;
using Newtonsoft.Json;
using Sciendo.Last.Fm.DataTypes;

namespace Sciendo.Topper.Source.DataTypes
{
    public class TopArtist:Item
    {
        [JsonProperty("playcount")]
        public int PlayCount { get; set; }

        [JsonProperty("@attr")]
        public TopArtistAdditionalAttribute AdditionalAttribute { get; set; }

        [JsonProperty("image")]
        public IEnumerable<Image> Images { get; set; }

        [JsonProperty("streamable")]
        public string Streamable { get; set; }



    }
}