using Microsoft.Extensions.Logging;
using Sciendo.OAuth1_0;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.MusicStory
{
    public class UrlProvider:IUrlProvider
    {
        private readonly ILogger<UrlProvider> logger;
        private readonly IOAuthUrlGenerator oauthUrlGenerator;
        private readonly MSConfig musicStoryConfig;

        public UrlProvider(ILogger<UrlProvider> logger, IOAuthUrlGenerator oauthUrlGenerator,MSConfig musicStoryConfig)
        {
            this.logger = logger;
            this.oauthUrlGenerator = oauthUrlGenerator??throw new ArgumentNullException(nameof(oauthUrlGenerator));
            this.musicStoryConfig = musicStoryConfig??throw new ArgumentNullException(nameof(musicStoryConfig));
        }

        private const string Template = "http://api.music-story.com/en/{0}/{1}{2}?{3}";

        public Uri GetUrl(string subject, ActionType actionType, string additionalParameters, long subjectId = 0, string attribute="")
        {
            logger.LogInformation("Building the url for {subject} and {actionType}", subject, actionType);
            if (string.IsNullOrEmpty(subject))
                throw new ArgumentNullException(nameof(subject));
            if (actionType==ActionType.None)
                throw new ArgumentException(nameof(actionType));
            if (actionType == ActionType.GetById && subjectId == 0)
                throw new ArgumentException(nameof(subjectId));
            var action = (actionType == ActionType.Search) ? "search" : subjectId.ToString();
            var formattedAttribute = (!string.IsNullOrEmpty(attribute) && !attribute.StartsWith("/")) ? "/" + attribute : attribute;
            var nonAuthenticatedUrl = string.Format(Template, subject, action, formattedAttribute, additionalParameters);
            logger.LogInformation("Non Authenticated url built: {nonAuthenticatedUrl}", nonAuthenticatedUrl);
            return oauthUrlGenerator.GenerateAuthenticatedUrl(new Uri(nonAuthenticatedUrl), 
                musicStoryConfig.ConsumerKey, 
                musicStoryConfig.CosumerSecret, 
                musicStoryConfig.AccessToken, 
                musicStoryConfig.TokenSecret, 
                "GET", 
                SignatureTypes.HMACSHA1);
        }
    }
}
