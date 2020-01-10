using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.MusicStory
{
    public interface IMusicStoryProvider<T>
    {
        T GetMusicStoryContent(Uri musicStoryUrl);
    }
}
