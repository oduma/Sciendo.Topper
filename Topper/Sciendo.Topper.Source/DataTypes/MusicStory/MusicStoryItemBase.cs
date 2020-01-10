using System.Xml.Serialization;

namespace Sciendo.Topper.Source.DataTypes.MusicStory
{
    public class MusicStoryItemBase
    {
        [XmlElement("id")]
        public long Id { get; set; }

        [XmlElement("update_date")]
        public string UpdateDate { get; set; }

        [XmlElement("creation_date")]
        public string CreationDate { get; set; }


    }
}