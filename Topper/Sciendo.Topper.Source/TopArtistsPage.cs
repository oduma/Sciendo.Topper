using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Sciendo.Last.Fm.DataTypes;

namespace Sciendo.Topper.Source
{
    public class TopArtistsPage
    {
        [JsonProperty("@attr")]
        public CollectionAdditionalAttributes AdditionalAttributes { get; set; }

        [JsonProperty("artist")]
        public TopArtist[] TopArtists { get; set; }
    }
}
