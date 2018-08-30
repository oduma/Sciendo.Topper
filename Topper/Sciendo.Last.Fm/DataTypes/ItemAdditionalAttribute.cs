using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Sciendo.Last.Fm.DataTypes
{
    public class ItemAdditionalAttribute
    {
        [JsonProperty("rank")]
        public int Rank { get; set; }
    }
}
