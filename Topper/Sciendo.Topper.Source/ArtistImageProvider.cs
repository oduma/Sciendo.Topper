using Microsoft.Extensions.Logging;
using Sciendo.MusicStory;
using Sciendo.Topper.Domain.Entities;
using Sciendo.Topper.Source.DataTypes.MusicStory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sciendo.Topper.Source
{
    public class ArtistImageProvider : IArtistImageProvider
    {
        private readonly ILogger<ArtistImageProvider> logger;
        private readonly MSConfig musicStoryConfig;
        private readonly IArtistsSummaryProvider artistsSummaryProvider;
        private readonly IPictureUrlsSummaryProvider pictureUrlsSummaryProvider;
        private readonly IMusicStoryProvider<byte[]> pictureProvider;

        public ArtistImageProvider(ILogger<ArtistImageProvider> logger, 
            MSConfig musicStoryConfig, 
            IArtistsSummaryProvider artistsSummaryProvider,
            IPictureUrlsSummaryProvider pictureUrlsSummaryProvider,
            IMusicStoryProvider<byte[]> pictureProvider)
        {
            this.logger = logger;
            this.musicStoryConfig = musicStoryConfig;
            this.artistsSummaryProvider = artistsSummaryProvider;
            this.pictureUrlsSummaryProvider = pictureUrlsSummaryProvider;
            this.pictureProvider = pictureProvider;
        }
        public NamedPicture GetImage(string artistName)
        {
            logger.LogDebug("Trying to get image for {artistName}", artistName);
            var artistsSummary = artistsSummaryProvider.Get(artistName);
            long? artistId = GetArtistId(artistsSummary);
            if(artistId.HasValue)
            {
                logger.LogDebug("Trying to get image for {artistId}", artistId);
                var pictureUrlsSummary = pictureUrlsSummaryProvider.Get(artistId.Value);
                string pictureUrl = GetPictureUrl(pictureUrlsSummary);
                if (!string.IsNullOrEmpty(pictureUrl))
                {
                    logger.LogDebug("Trying to get image from {pictureUrl}", pictureUrl);
                    return new NamedPicture { Picture = pictureProvider.GetMusicStoryContent(new Uri(pictureUrl)), 
                        Extension = GetExtensionFromUrl(pictureUrl), Name = artistName };
                }
            }
            logger.LogDebug("No image found for {artistName}", artistName);
            return null;
            
        }

        private string GetExtensionFromUrl(string pictureUrl)
        {
            var pictureUrlParts = pictureUrl.Split(new char[] { '.' });
            return pictureUrlParts[pictureUrlParts.Length - 1];

        }

        private string GetPictureUrl(PictureUrlsSummary pictureUrlsSummary)
        {
            if (pictureUrlsSummary.Code == -1)
            {
                logger.LogDebug("No picture url found for artist.");
                return null;
            }
            var selectedPictureUrl = pictureUrlsSummary.PictureUrlsSummaryCollection.OrderBy((p) => p.Height * p.Width).LastOrDefault();
            logger.LogDebug("Picture Url found.");
            return selectedPictureUrl.Url400;
        }

        private long? GetArtistId(ArtistsSummary artistsSummary)
        {
            if (artistsSummary.Code == -1)
            {
                logger.LogDebug("No artists found");
                return null;
            }
            if (artistsSummary.ArtistsSummaryCollection[0].SearchScores.SearchScore < 100)
            {
                logger.LogDebug("No artists found as precise matches");
                return null;
            }
            logger.LogDebug("One Artist Id selected.");
            return artistsSummary.ArtistsSummaryCollection[0].Id;
        }
    }
}
