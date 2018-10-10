using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sciendo.Last.Fm;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Source
{
    public static class TopItemsSourcer
    {
        public static List<TopItem> GetTodayTopItems(LastFmConfig topperLastFmConfig)
        {
            var lastFmTopArtistSourcer = new LastFmTopArtistSourcer(
                new ContentProvider<TopArtistsRootObject>(new UrlProvider(topperLastFmConfig.ApiKey),
                    new LastFmProvider()));
            var todayTopItems = lastFmTopArtistSourcer.GetTopItems(topperLastFmConfig.UserName);
            var lastFmLovedSourcer = new LastFmLovedSourcer(
                new ContentProvider<LovedTracksRootObject>(new UrlProvider(topperLastFmConfig.ApiKey),
                    new LastFmProvider()));
            foreach (var lovedTopItem in lastFmLovedSourcer.GetTopItems(topperLastFmConfig.UserName))
            {
                var topItem = todayTopItems.FirstOrDefault(t => t.Name == lovedTopItem.Name);

                if (topItem != null)
                {
                    topItem.Loved = lovedTopItem.Loved;
                }
                else
                {
                    todayTopItems.Add(lovedTopItem);
                }
            }

            return todayTopItems;
        }


    }
}
