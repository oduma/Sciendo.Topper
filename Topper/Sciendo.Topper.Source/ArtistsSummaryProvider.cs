using Microsoft.Extensions.Logging;
using Sciendo.MusicStory;
using Sciendo.Topper.Source.DataTypes.MusicStory;

namespace Sciendo.Topper.Source
{
    public class ArtistsSummaryProvider : IArtistsSummaryProvider
    {
        private readonly ILogger<ArtistsSummaryProvider> logger;
        private readonly IContentProvider<ArtistsSummary> contentProvider;

        private const string Subject = "artist";

        private readonly ActionType ActionType = ActionType.Search;

        public ArtistsSummaryProvider(ILogger<ArtistsSummaryProvider> logger, IContentProvider<ArtistsSummary> contentProvider)
        {
            this.logger = logger;
            this.contentProvider = contentProvider;
        }
        public ArtistsSummary Get(string artistName)
        {
            logger.LogDebug("Getting artist {artistName} from musicstory.", artistName);
            return contentProvider.GetContent(Subject, ActionType, $"name={artistName}");
        }
    }
}
