using System;

namespace Sciendo.Last.Fm
{
    public interface IUrlProvider
    {
        string ApiKey { get; }

        Uri GetUrl(string methodName, string userName, int pageNumber=1, string additionalParameters ="" );
    }
}