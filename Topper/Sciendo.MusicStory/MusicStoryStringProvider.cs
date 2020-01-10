using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sciendo.MusicStory
{
    public class MusicStoryStringProvider : IMusicStoryProvider<string>
    {
        private readonly ILogger<MusicStoryStringProvider> logger;

        public MusicStoryStringProvider(ILogger<MusicStoryStringProvider> logger)
        {
            this.logger = logger;
        }
        public string GetMusicStoryContent(Uri musicStroyUri)
        {
            logger.LogInformation("Getting content from MusicStory...");
            if (musicStroyUri == null)
                throw new ArgumentNullException(nameof(musicStroyUri));
            var httpClient = new HttpClient();
            try
            {
                using (var getTask = httpClient.GetStringAsync(musicStroyUri)
                    .ContinueWith(p => p).Result)
                {
                    if (getTask.Status == TaskStatus.RanToCompletion || !string.IsNullOrEmpty(getTask.Result))
                    {
                        logger.LogInformation("Content retrieved Ok from MusicStory");
                        return getTask.Result;
                    }
                    logger.LogWarning("Content not retrieved from MusicStory. Task returned {getTask.Status}", getTask.Status);
                    return string.Empty;
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Cannot retrieve content from MusicStory.");
                return string.Empty;
            }

        }
    }
}
