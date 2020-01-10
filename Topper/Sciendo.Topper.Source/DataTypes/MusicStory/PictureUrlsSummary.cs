using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Sciendo.Topper.Source.DataTypes.MusicStory
{
    [XmlRoot("root")]
    public class PictureUrlsSummary:MusicStoryResponseBase
    {
        [XmlArray("data")]
        [XmlArrayItem("item")]
        public PictureUrlSummary[] PictureUrlsSummaryCollection { get; set; }
    }
}
