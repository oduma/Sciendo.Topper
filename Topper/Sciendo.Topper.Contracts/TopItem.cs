using System;
using Newtonsoft.Json;

namespace Sciendo.Topper.Contracts
{
    public class TopItem:BaseTopItem
    {
        [JsonProperty("day")]
        public DateTime Date { get; set; }
    }
}
