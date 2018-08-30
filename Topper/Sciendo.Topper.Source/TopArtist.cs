using System.Collections.Generic;
using System.Net.Mime;
using Newtonsoft.Json;
using Sciendo.Last.Fm.DataTypes;

namespace Sciendo.Topper.Source
{
    public class TopArtist:Artist
    {
        [JsonProperty("streamable")]
        public string Streamable { get; set; }

        [JsonProperty("image")]
        public IEnumerable<Image> Images { get; set; }

        [JsonProperty("playcount")]
        public int PlayCount { get; set; }

        [JsonProperty("@attr")]
        public ItemAdditionalAttribute AdditionalAttribute { get; set; }


    }
}