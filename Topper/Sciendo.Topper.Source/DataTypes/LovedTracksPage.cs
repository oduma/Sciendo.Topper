using Newtonsoft.Json;
using Sciendo.Last.Fm.DataTypes;

namespace Sciendo.Topper.Source.DataTypes
{
    public class LovedTracksPage
    {
        [JsonProperty("@attr")]
        public CollectionAdditionalAttributes AdditionalAttributes { get; set; }

        [JsonProperty("track")]
        public LovedTrack[] LovedTracks { get; set; }

    }
}