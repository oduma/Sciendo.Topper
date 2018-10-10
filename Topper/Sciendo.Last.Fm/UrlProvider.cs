using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Last.Fm
{
    public class UrlProvider:IUrlProvider
    {
        private readonly string _apiKey;

        public UrlProvider(string apiKey)
        {
            _apiKey = apiKey;
        }
        private const string Template = "http://ws.audioscrobbler.com/2.0/?method={0}&user={1}&api_key={2}&page={3}{4}&format=json";
        public Uri GetUrl(string methodName, string userName, int pageNumber = 1, string additionalParameters ="")
        {
            var formattedParams = (string.IsNullOrEmpty(additionalParameters)) ? "" : $"&{additionalParameters}";
           return new Uri(string.Format(Template,methodName,userName,_apiKey,pageNumber,formattedParams));
        }
    }
}
