using System;
using System.Collections.Generic;
using System.Linq;
using Sciendo.Last.Fm;
using Sciendo.Topper.Contracts;
using Sciendo.Topper.Source.DataTypes;

namespace Sciendo.Topper.Source
{
    public class LastFmTopArtistsProvider:LastFmProviderBase<TopArtistsRootObject>
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
            int rank = 1;
            List<TopItem> results= new List<TopItem>();
            foreach (var playCount in topArtists.Select(a => a.PlayCount).Distinct().OrderByDescending(c => c).Take(3))
            {
                results.AddRange(topArtists.Where(ta => ta.PlayCount == playCount)
                    .Select(ta => new TopItem
                        {Hits = ta.PlayCount, Name = ta.Name, Date = DateTime.Now, DayRanking = rank}));
                rank++;
            }

            return results;
        }

        public override void MergeSourceProperties(TopItem fromItem, TopItem toItem)
        {
            if(fromItem==null)
                throw new ArgumentNullException(nameof(fromItem));
            if(toItem==null)
                throw new ArgumentNullException(nameof(toItem));
            toItem.Hits = fromItem.Hits;
        }

        public LastFmTopArtistsProvider(IContentProvider<TopArtistsRootObject> contentProvider) : base(contentProvider)
        {
        }
    }
}
