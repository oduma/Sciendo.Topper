using System.Xml.Serialization;

namespace Sciendo.Topper.Source.DataTypes.MusicStory
{
    public class PictureUrlSummary:MusicStoryItemBase
    {
        
        [XmlElement("source_id")]
        public long SourceId { get; set; }

        [XmlElement("source")]
        public string Source { get; set; }

        [XmlElement("width")]
        public int Width { get; set; }

        [XmlElement("height")]
        public int Height { get; set; }

        [XmlElement("url")]
        public string Url { get; set; }

        [XmlElement("url_400")]
        public string Url400 { get; set; }

        [XmlElement("mime_type")]
        public string MimeType { get; set; }

        [XmlElement("copyright")]
        public string Copyright { get; set; }

        [XmlElement("licence")]
        public string Licence { get; set; }

        [XmlElement("main")]
        public string Main { get; set; }
    }
}