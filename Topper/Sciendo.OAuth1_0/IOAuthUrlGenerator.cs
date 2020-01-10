using System;

namespace Sciendo.OAuth1_0
{
    public interface IOAuthUrlGenerator
    {
        Uri GenerateAuthenticatedUrl(Uri url, 
            string consumerKey, 
            string consumerSecret, 
            string token, 
            string tokenSecret, 
            string httpMethod, 
            SignatureTypes signatureType);
    }
}