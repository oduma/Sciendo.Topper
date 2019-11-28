using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Sciendo.Last.Fm;
using Sciendo.Last.Fm.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Source.DataTypes;

namespace Sciendo.Topper.Source
{
    public class LastFmLovedProvider:LastFmProviderBase<LovedTracksRootObject>
    {
        private readonly ILogger<LastFmLovedProvider> logger;

        private bool IsToday(LastFmDate date)
        {
            DateTimeOffset dateTimeOffset=DateTimeOffset.FromUnixTimeSeconds(date.Uts);
            if (dateTimeOffset.UtcDateTime >= DateTime.UtcNow.AddDays(-1))
                return true;
            return false;
        }

        public LastFmLovedProvider(ILogger<LastFmLovedProvider> logger, IContentProvider<LovedTracksRootObject> contentProvider) : base(contentProvider)
        {
            this.logger = logger;
        }

        protected override string LastFmMethod
        {
            get => "user.getlovedtracks";
        }

        protected override string AdditionalParameters { get; }

        public override List<TopItem> GetItems(string userName)
        {
            logger.LogInformation("Getting Loved Artists with {LastFmMethod}", LastFmMethod);
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            var lovedTracksRoot = ContentProvider.GetContent(LastFmMethod, userName);
            if (lovedTracksRoot == null)
            {
                logger.LogWarning("No loved Tracks from last.fm");
                return new List<TopItem>();
            }

            if (lovedTracksRoot.LovedTracksPage == null)
            {
                logger.LogWarning("No loved Tracks page from last.fm");
                return new List<TopItem>();
            }

            if (lovedTracksRoot.LovedTracksPage.LovedTracks == null)
            {
                logger.LogWarning("No Tracks on Loved Tracks Page from last.fm");
                return new List<TopItem>();
            }

            var lovedTracks = lovedTracksRoot.LovedTracksPage.LovedTracks
                .Where(l => IsToday(l.Date)).ToList();
            if (!lovedTracks.Any())
            {
                logger.LogWarning("No loved Tracks for today.");
                return new List<TopItem>();
            }

            var lovedArtists= lovedTracks.GroupBy(l => l.Artist.Name).Select(g => new TopItem
                    { Hits = 0, Name = g.Key, Date = DateTime.Now, Loved = g.Count() })
                .ToList();
            logger.LogInformation("Retrieved {lovedArtists.Count} loved artists.", lovedArtists.Count);
            return lovedArtists;

        }

        public override void MergeSourceProperties(TopItem fromItem, TopItem toItem)
        {
            if (fromItem == null)
                throw new ArgumentNullException(nameof(fromItem));
            if (toItem == null)
                throw new ArgumentNullException(nameof(toItem));

            toItem.Loved = fromItem.Loved;
            logger.LogInformation("After merge top Item has been set to {toItem.Loved} Loved.", toItem.Loved);
        }
    }
}
