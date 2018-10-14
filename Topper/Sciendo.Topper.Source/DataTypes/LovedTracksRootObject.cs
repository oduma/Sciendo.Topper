using Newtonsoft.Json;

namespace Sciendo.Topper.Source.DataTypes
{
    public class LovedTracksRootObject
    {
        [JsonProperty("lovedtracks")]
        public LovedTracksPage LovedTracksPage { get; set; }

    }
}