using System.Xml.Serialization;

namespace Sciendo.Topper.Source.DataTypes.MusicStory
{
    public abstract class MusicStoryResponseBase
    {
        [XmlElement("script")]
        public MusicStoryScript MusicStoryScript { get; set; }

        [XmlElement("version")]
        public string Version { get; set; }

        [XmlElement("code")]
        public int Code { get; set; }

        [XmlElement("count")]
        public int Count { get; set; }

        [XmlElement("pageCount")]
        public int PageCount { get; set; }

        [XmlElement("currentPage")]
        public int CurrentPage { get; set; }

    }
}