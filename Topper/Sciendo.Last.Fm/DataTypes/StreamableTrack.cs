using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Sciendo.Last.Fm.DataTypes
{
    public class StreamableTrack
    {
        [JsonProperty("#text")]

        public string Text { get; set; }

        [JsonProperty("fulltrack")]
        public string FullTrack { get; set; }
    }
}
