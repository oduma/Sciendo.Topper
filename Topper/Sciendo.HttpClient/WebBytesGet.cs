using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.Web
{
    public class WebBytesGet : IWebGet<byte[]>
    {
        private readonly ILogger<WebBytesGet> logger;

        public WebBytesGet(ILogger<WebBytesGet> logger)
        {
            this.logger = logger;
        }

        public byte[] Get(Uri url)
        {
            logger.LogInformation("Getting content from Url...");
            if (url == null)
                throw new ArgumentNullException(nameof(url));
            var httpClient = new HttpClient();
            try
            {
                using (var getTask = httpClient.GetByteArrayAsync(url)
                    .ContinueWith(p => p).Result)
                {
                    if (getTask.Status == TaskStatus.RanToCompletion || getTask.Result == null)
                    {
                        logger.LogInformation("Content retrieved Ok from url");
                        return getTask.Result;
                    }
                    logger.LogWarning("Content not retrieved from Url. Task returned {getTask.Status}", getTask.Status);
                    return null;
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Cannot retrieve content from Url.");
                return null;
            }

        }
    }
}
