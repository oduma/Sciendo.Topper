using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Sciendo.Last.Fm.DataTypes
{
    public class Item
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("mbid")]
        public string Mbid { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
