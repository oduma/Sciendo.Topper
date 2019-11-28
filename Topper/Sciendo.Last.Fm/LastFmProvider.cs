using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sciendo.Last.Fm
{
    public class LastFmProvider:ILastFmProvider
    {
        private readonly ILogger<LastFmProvider> logger;

        public LastFmProvider(ILogger<LastFmProvider> logger)
        {
            this.logger = logger;
        }
        public string GetLastFmContent(Uri lastFmUri)
        {
            logger.LogInformation("Getting content from last.fm...");
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
                        logger.LogInformation("Content retrieved Ok from last.fm");
                        return getTask.Result;
                    }
                    logger.LogWarning("Content not retrieved from last.fm. Task returned {getTask.Status}",getTask.Status);
                    return string.Empty;
                }
            }
            catch(Exception e)
            {
                logger.LogError(e,"Cannot retrieve content from last.fm.");
                return string.Empty;
            }

        }
    }
}
