using System.Xml.Serialization;

namespace Sciendo.Topper.Source.DataTypes.MusicStory
{
    public class SearchScores
    {
        [XmlElement("name")]
        public double SearchScore { get; set; }

    }
}