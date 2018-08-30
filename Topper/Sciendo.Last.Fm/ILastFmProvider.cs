using System;

namespace Sciendo.Last.Fm
{
    public interface ILastFmProvider
    {
        string GetLastFmContent(Uri lastFmUri);
    }
}