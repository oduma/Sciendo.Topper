using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sciendo.Last.Fm
{
    public class LastFmProvider:ILastFmProvider
    {
        public string GetLastFmContent(Uri lastFmUri)
        {
            if(lastFmUri==null)
                throw new ArgumentNullException(nameof(lastFmUri));
            var httpClient = new HttpClient();
            try
            {
                using (var getTask = httpClient.GetStringAsync(lastFmUri)
                    .ContinueWith(p => p).Result)
                {
                    if (getTask.Status == TaskStatus.RanToCompletion || !string.IsNullOrEmpty(getTask.Result))
                    {
                        return getTask.Result;
                    }
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }

        }
    }
}
