using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Sciendo.Topper.Contracts
{
    public class BaseTopItem
    {
        [JsonProperty("artist")]
        public string Name { get; set; }

        [JsonProperty("hits")]
        public int Hits { get; set; }


    }
}
