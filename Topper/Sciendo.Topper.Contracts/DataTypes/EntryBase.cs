using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Contracts.DataTypes
{
    public class EntryBase
    {
        public EntryBase(string name, string pictureUrl)
        {
            Name = name;
            PictureUrl = pictureUrl;
        }
        public string Name { get; private set; }

        public string PictureUrl { get; private set; }

    }
}
