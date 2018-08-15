using System;
using System.Collections.Generic;
using System.Text;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Source
{
    public class LastFmSourcer
    {
        private readonly string _siteUrl;

        public LastFmSourcer(string siteUrl)
        {
            _siteUrl = siteUrl;
        }
        public TopItem[] GetTopItems(string userName)
        {
            throw new NotImplementedException();
        }
    }
}
