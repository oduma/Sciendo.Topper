using System;
using Serilog;

namespace Sciendo.Last.Fm
{
    public class UrlProvider:IUrlProvider
    {
        private readonly string _apiKey;

        public UrlProvider(string apiKey)
        {
            if(string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException(nameof(apiKey));
            _apiKey = apiKey;
        }
        private const string Template = "http://ws.audioscrobbler.com/2.0/?method={0}&user={1}&api_key={2}&page={3}{4}&format=json";
        public Uri GetUrl(string methodName, string userName, int pageNumber = 1, string additionalParameters ="")
        {
            Log.Information("Building the url for {methodName}", methodName);
            if(string.IsNullOrEmpty(methodName))
                throw new ArgumentNullException(nameof(methodName));
            if(string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            var formattedParams = (string.IsNullOrEmpty(additionalParameters)) ? "" : $"&{additionalParameters}";
            var urlWithoutApiKey = string.Format(Template, methodName, userName, "masked_api_key", pageNumber, formattedParams);
            Log.Information("Url built: {urlWithoutApiKey}", urlWithoutApiKey);
           return new Uri(string.Format(Template,methodName,userName,_apiKey,pageNumber,formattedParams));
        }
    }
}
