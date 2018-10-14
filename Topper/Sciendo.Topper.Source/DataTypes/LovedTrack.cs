using Newtonsoft.Json;
using Sciendo.Last.Fm.DataTypes;

namespace Sciendo.Topper.Source.DataTypes
{
    public class LovedTrack:Track
    {
        [JsonProperty("date")]
        public LastFmDate Date { get; set; }
    }
}