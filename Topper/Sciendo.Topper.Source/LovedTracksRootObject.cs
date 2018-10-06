using Newtonsoft.Json;

namespace Sciendo.Topper.Source
{
    public class LovedTracksRootObject
    {
        [JsonProperty("lovedtracks")]
        public LovedTracksPage LovedTracksPage { get; set; }

    }
}