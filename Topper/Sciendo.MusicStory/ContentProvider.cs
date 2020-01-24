using Microsoft.Extensions.Logging;
using Sciendo.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Sciendo.MusicStory
{
    public class ContentProvider<T> : IContentProvider<T> where T : class, new()
    {
        private readonly ILogger<ContentProvider<T>> logger;
        private readonly IUrlProvider urlProvider;
        private readonly IWebGet<string> musicStoryProvider;

        public ContentProvider(ILogger<ContentProvider<T>> logger, IUrlProvider urlProvider, IWebGet<string> musicStoryProvider)
        {
            this.logger = logger;
            this.urlProvider = urlProvider??throw new ArgumentNullException(nameof(urlProvider));
            this.musicStoryProvider = musicStoryProvider??throw new ArgumentNullException(nameof(musicStoryProvider));
        }
        public T GetContent(string subject, ActionType actionType, string additionalParameters, long subjectId = 0, string attribute="")
        {
            logger.LogInformation("Getting content from music.story using {subject} and {actionType}", subject, actionType);
            if (string.IsNullOrEmpty(subject))
                throw new ArgumentNullException(nameof(subject));
            if (actionType==ActionType.None)
                throw new ArgumentException(nameof(actionType));
            if (actionType == ActionType.GetById && subjectId == 0)
                throw new ArgumentException(nameof(subjectId));

            var url = urlProvider.GetUrl(subject,actionType,additionalParameters,subjectId,attribute);
            var rawData = musicStoryProvider.Get(url);
            if (string.IsNullOrEmpty(rawData))
            {
                logger.LogWarning("No content or empty content returned from music story.");
                return null;
            }
            logger.LogInformation("Content retrieved Ok from musicStory.");
            var xmlSerialzer = new XmlSerializer(typeof(T));
            var encoding = new UTF8Encoding();
            using (var s = new MemoryStream(encoding.GetBytes(rawData)))
                return xmlSerialzer.Deserialize(s) as T;
        }
    }
}
