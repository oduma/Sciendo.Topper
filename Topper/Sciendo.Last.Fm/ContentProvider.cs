using System;
using Serilog;

namespace Sciendo.Last.Fm
{
    public class ContentProvider<T> : IContentProvider<T> where T: class, new()
    {
        private readonly IUrlProvider _urlProvider;
        private readonly ILastFmProvider _lastFmProvider;

        public ContentProvider(IUrlProvider urlProvider, ILastFmProvider lastFmProvider)
        {
            _urlProvider = urlProvider ?? throw new ArgumentNullException(nameof(urlProvider));
            _lastFmProvider = lastFmProvider ?? throw new ArgumentNullException(nameof(lastFmProvider));
        }

        public T GetContent(string methodName, string userName, int currentPage = 1, string additionalParameters="")
        {
            Log.Information("Getting content from last.fm using {methodName}", methodName);
            if(string.IsNullOrEmpty(methodName))
                throw new ArgumentNullException(nameof(methodName));
            if(string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            var url = _urlProvider.GetUrl(methodName,userName,currentPage,additionalParameters);
            var rawData = _lastFmProvider.GetLastFmContent(url);
            if (string.IsNullOrEmpty(rawData))
            {
                Log.Warning("No content or empty content returned from las.fm.");
                return new T();
            }
            Log.Information("Content retrieved Ok from last.fm.");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(rawData);
        }
    }
}
