using System;

namespace Sciendo.MusicStory
{
    public interface IUrlProvider
    {
        Uri GetUrl(string subject, ActionType actionType, string additionalParameters, long subjectId = 0, string attribute="");
    }
}