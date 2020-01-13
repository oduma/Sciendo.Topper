using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sciendo.MusicStory;
using Sciendo.OAuth1_0;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Tests.Unit
{
    [TestClass]
    public class GenerateOAuth1_0SignatureTests
    {
        [TestMethod]
        public void GenerateSignature_Ok()
        {
            OAuthUrlGenerator signatureGenerator = new OAuthUrlGenerator();
            var uri = new Uri("http://api.music-story.com/en/artist/search?name=Pumarosa");
            var musicStoryConfig = new MSConfig { AccessToken = "a", ConsumerKey = "b", CosumerSecret = "d", TokenSecret = "e" };
            var signature = signatureGenerator.GenerateAuthenticatedUrl(uri, musicStoryConfig.ConsumerKey, musicStoryConfig.CosumerSecret, musicStoryConfig.AccessToken, musicStoryConfig.TokenSecret, "GET", SignatureTypes.HMACSHA1);
            Assert.IsNotNull(signature);

        }
    }
}
