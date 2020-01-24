using Microsoft.Extensions.Logging;
using Sciendo.Web;
using System;

namespace Sciendo.Last.Fm
{
    public class ContentProvider<T> : IContentProvider<T> where T: class, new()
    {
        private readonly ILogger<ContentProvider<T>> logger;
        private readonly IUrlProvider _urlProvider;
        private readonly IWebGet<string> _lastFmProvider;

        public ContentProvider(ILogger<ContentProvider<T>> logger, IUrlProvider urlProvider, IWebGet<string> lastFmProvider)
        {
            this.logger = logger;
            _urlProvider = urlProvider ?? throw new ArgumentNullException(nameof(urlProvider));
            _lastFmProvider = lastFmProvider ?? throw new ArgumentNullException(nameof(lastFmProvider));
        }

        public T GetContent(string methodName, string userName, int currentPage = 1, string additionalParameters="")
        {
            logger.LogInformation("Getting content from last.fm using {methodName}", methodName);
            if(string.IsNullOrEmpty(methodName))
                throw new ArgumentNullException(nameof(methodName));
            if(string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            var url = _urlProvider.GetUrl(methodName,userName,currentPage,additionalParameters);
            var rawData = _lastFmProvider.Get(url);
            if (string.IsNullOrEmpty(rawData))
            {
                logger.LogWarning("No content or empty content returned from las.fm.");
                return new T();
            }
            logger.LogInformation("Content retrieved Ok from last.fm.");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(rawData);
        }
    }
}
