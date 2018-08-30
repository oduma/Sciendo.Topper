using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sciendo.Last.Fm;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Source
{
    public class LastFmTopArtistSourcer
    {
        private readonly IContentProvider<RootObject> _contentProvider;

        public LastFmTopArtistSourcer(IContentProvider<RootObject> contentProvider )
        {
            _contentProvider = contentProvider;
        }
        public TopItem[] GetTopItems(string userName)
        {
            var topArtists = _contentProvider.GetContent("user.gettopartists", userName, 1, "period=7day").TopArtistsPage.TopArtists;
            var maxPlayCount = topArtists.First().PlayCount;
            return
                topArtists.Where(ta => ta.PlayCount == maxPlayCount)
                    .Select(ta => new TopItem {Hits = ta.PlayCount, Name = ta.Name, Date = DateTime.Now})
                    .ToArray();
        }
    }
}
