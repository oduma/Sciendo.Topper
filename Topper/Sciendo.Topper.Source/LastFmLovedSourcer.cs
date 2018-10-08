using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sciendo.Last.Fm;
using Sciendo.Last.Fm.DataTypes;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Source
{
    public class LastFmLovedSourcer
    {
        private readonly IContentProvider<LovedTracksRootObject> _contentProvider;

        public LastFmLovedSourcer(IContentProvider<LovedTracksRootObject> contentProvider)
        {
            _contentProvider = contentProvider;
        }
        public List<TopItem> GetTopItems(string userName)
        {
            var lovedTracks = _contentProvider.GetContent("user.getlovedtracks", userName).LovedTracksPage.LovedTracks
                .Where(l => IsToday(l.Date));
            if(!lovedTracks.Any())
                return new List<TopItem>();

            return
                lovedTracks.GroupBy(l => l.Artist.Name).Select(g => new TopItem
                        {Hits = 0, Name = g.Key, Date = DateTime.Now, Loved = g.Count()})
                    .ToList();
        }

        private bool IsToday(LastFmDate date)
        {
            DateTimeOffset dateTimeOffset=DateTimeOffset.FromUnixTimeSeconds(date.Uts);
            if (dateTimeOffset.UtcDateTime >= DateTime.UtcNow.AddDays(-1))
                return true;
            return false;
        }
    }
}
