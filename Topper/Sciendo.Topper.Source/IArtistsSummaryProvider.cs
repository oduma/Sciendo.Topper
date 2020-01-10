using Sciendo.Topper.Source.DataTypes.MusicStory;

namespace Sciendo.Topper.Source
{
    public interface IArtistsSummaryProvider
    {
        ArtistsSummary Get(string artistName);
    }
}