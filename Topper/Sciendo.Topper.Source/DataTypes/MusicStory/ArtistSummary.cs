using System.Xml.Serialization;

namespace Sciendo.Topper.Source.DataTypes.MusicStory
{
    public class ArtistSummary:MusicStoryItemBase
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("ipi")]
        public string Ipi { get; set; }

        [XmlElement("type")]
        public string Type { get; set; }

        [XmlElement("url")]
        public string Url { get; set; }

        [XmlElement ("firstname")]
        public string FirstName { get; set; }

        [XmlElement("lastname")]
        public string LastName { get; set; }

        [XmlElement("search_scores")]
        public SearchScores SearchScores { get; set; }
    }
}