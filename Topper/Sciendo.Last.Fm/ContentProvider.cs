namespace Sciendo.Last.Fm
{
    public class ContentProvider<T> : IContentProvider<T> where T: class, new()
    {
        private readonly IUrlProvider _urlProvider;
        private readonly ILastFmProvider _lastFmProvider;

        public ContentProvider(IUrlProvider urlProvider, ILastFmProvider lastFmProvider)
        {
            _urlProvider = urlProvider;
            _lastFmProvider = lastFmProvider;
        }

        public T GetContent(string methodName, string userName, int currentPage = 1, string additionalParameters="")
        {
            var url = _urlProvider.GetUrl(methodName,userName,currentPage,additionalParameters);
            var rawData = _lastFmProvider.GetLastFmContent(url);
            if (string.IsNullOrEmpty(rawData))
                return new T();

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(rawData);
        }
    }
}
