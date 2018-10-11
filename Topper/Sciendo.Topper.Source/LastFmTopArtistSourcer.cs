using System;
using System.Collections.Generic;
using System.Linq;
using Sciendo.Last.Fm;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Source
{
    public class LastFmTopArtistSourcer:LastFmSourcerBase<TopArtistsRootObject>
    {

        protected override string LastFmMethod { get=> "user.gettopartists"; }
        protected override string AdditionalParameters
        {
            get => "period=7day";
        }

        public override List<TopItem> GetItems(string userName)
        {
            if(string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            var topArtistsRoot = ContentProvider.GetContent(LastFmMethod, userName, 1, AdditionalParameters);
            if(topArtistsRoot==null)
                return new List<TopItem>();
            if(topArtistsRoot.TopArtistsPage==null)
                return new List<TopItem>();
            if(topArtistsRoot.TopArtistsPage.TopArtists==null)
                return new List<TopItem>();
            var topArtists = topArtistsRoot
                .TopArtistsPage
                .TopArtists;
            var maxPlayCount = topArtists.First().PlayCount;
            return
                topArtists.Where(ta => ta.PlayCount == maxPlayCount)
                    .Select(ta => new TopItem { Hits = ta.PlayCount, Name = ta.Name, Date = DateTime.Now })
                    .ToList();
        }

        public override void MergeSourceProperties(TopItem fromItem, TopItem toItem)
        {
            toItem.Hits = fromItem.Hits;
        }

        public LastFmTopArtistSourcer(IContentProvider<TopArtistsRootObject> contentProvider) : base(contentProvider)
        {
        }
    }
}
