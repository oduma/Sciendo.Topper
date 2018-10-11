using System;
using System.Collections.Generic;
using System.Linq;
using Sciendo.Last.Fm;
using Sciendo.Last.Fm.DataTypes;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Source
{
    public class LastFmLovedSourcer:LastFmSourcerBase<LovedTracksRootObject>
    {
        private bool IsToday(LastFmDate date)
        {
            DateTimeOffset dateTimeOffset=DateTimeOffset.FromUnixTimeSeconds(date.Uts);
            if (dateTimeOffset.UtcDateTime >= DateTime.UtcNow.AddDays(-1))
                return true;
            return false;
        }

        public LastFmLovedSourcer(IContentProvider<LovedTracksRootObject> contentProvider) : base(contentProvider)
        {
        }

        protected override string LastFmMethod
        {
            get => "user.getlovedtracks";
        }

        protected override string AdditionalParameters { get; }

        public override List<TopItem> GetItems(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            var lovedTracksRoot = ContentProvider.GetContent(LastFmMethod, userName);
            if (lovedTracksRoot == null)
                return new List<TopItem>();
            if (lovedTracksRoot.LovedTracksPage == null)
                return new List<TopItem>();
            if (lovedTracksRoot.LovedTracksPage.LovedTracks == null)
                return new List<TopItem>();

            var lovedTracks = lovedTracksRoot.LovedTracksPage.LovedTracks
                .Where(l => IsToday(l.Date)).ToList();
            if (!lovedTracks.Any())
                return new List<TopItem>();

            return
                lovedTracks.GroupBy(l => l.Artist.Name).Select(g => new TopItem
                        { Hits = 0, Name = g.Key, Date = DateTime.Now, Loved = g.Count() })
                    .ToList();
        }

        public override void MergeSourceProperties(TopItem fromItem, TopItem toItem)
        {
            toItem.Loved = fromItem.Loved;
        }
    }
}
