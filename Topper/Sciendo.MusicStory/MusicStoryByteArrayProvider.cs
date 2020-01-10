using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sciendo.MusicStory
{
    public class MusicStoryByteArrayProvider : IMusicStoryProvider<byte[]>
    {
        private readonly ILogger<MusicStoryByteArrayProvider> logger;

        public MusicStoryByteArrayProvider(ILogger<MusicStoryByteArrayProvider> logger)
        {
            this.logger = logger;
        }
        public byte[] GetMusicStoryContent(Uri musicStroyUri)
        {
            logger.LogInformation("Getting content from MusicStory...");
            if (musicStroyUri == null)
                throw new ArgumentNullException(nameof(musicStroyUri));
            var httpClient = new HttpClient();
            try
            {
                using (var getTask = httpClient.GetByteArrayAsync(musicStroyUri)
                    .ContinueWith(p => p).Result)
                {
                    if (getTask.Status == TaskStatus.RanToCompletion || getTask.Result==null)
                    {
                        logger.LogInformation("Content retrieved Ok from MusicStory");
                        return getTask.Result;
                    }
                    logger.LogWarning("Content not retrieved from MusicStory. Task returned {getTask.Status}", getTask.Status);
                    return null;
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Cannot retrieve content from MusicStory.");
                return null;
            }

        }
    }
}
