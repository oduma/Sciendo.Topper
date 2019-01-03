using System;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;

namespace Sciendo.Last.Fm
{
    public class LastFmProvider:ILastFmProvider
    {
        public string GetLastFmContent(Uri lastFmUri)
        {
            Log.Information("Getting content from last.fm...");
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
                        Log.Information("Content retrieved Ok from last.fm");
                        return getTask.Result;
                    }
                    Log.Warning("Content not retrieved from last.fm. Task returned {getTask.Status}",getTask.Status);
                    return string.Empty;
                }
            }
            catch(Exception e)
            {
                Log.Error(e,"Cannot retrieve content from last.fm.");
                return string.Empty;
            }

        }
    }
}
