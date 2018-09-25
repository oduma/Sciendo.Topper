using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Sciendo.Topper.Contracts
{
    public class TopItemWithScore: TopItem
    {
        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("year")]
        public string Year { get; set; }
    }
}
