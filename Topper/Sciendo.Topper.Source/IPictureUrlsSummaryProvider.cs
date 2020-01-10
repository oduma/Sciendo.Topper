using Sciendo.Topper.Source.DataTypes.MusicStory;

namespace Sciendo.Topper.Source
{
    public interface IPictureUrlsSummaryProvider
    {
        PictureUrlsSummary Get(long artistId);
    }
}