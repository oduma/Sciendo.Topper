using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Sciendo.Last.Fm.DataTypes
{
    public class LastFmDate
    {
        [JsonProperty("uts")]
        public long Uts { get; set; }

        [JsonProperty("#text")]

        public string Text { get; set; }
    }
}
