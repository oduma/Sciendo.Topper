using Sciendo.MusicStory;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Source
{
    public interface IArtistImageProvider
    {
        NamedPicture GetImage(string artistName);
    }
}
