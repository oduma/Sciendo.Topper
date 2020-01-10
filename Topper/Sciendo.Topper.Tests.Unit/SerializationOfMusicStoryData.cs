using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sciendo.Topper.Source.DataTypes.MusicStory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Sciendo.Topper.Tests.Unit
{
    [TestClass]
    public class SerializationOfMusicStoryData
    {
        [TestMethod]
        public void Serialize_ArtistsSummary_Ok()
        {
            string source= @"<root>
<script id=""tinyhippos - injected""/>
  <version> 1.29 </version>
  <code > 0 </code >
  <count > 394 </count >
  <pageCount > 40 </pageCount >
  <currentPage > 1 </currentPage >
  <data >
  <item >
  <id > 121004 </id >
  <name > George </name >
  <ipi />
  <type > Band </type >
  <url > http://data.music-story.com/george-suisse</url>
<firstname />
<lastname />
<update_date > 2017 - 03 - 13 17:01:04 </update_date >
       <creation_date > 2010 - 06 - 24 00:00:00 </creation_date >
              <search_scores >
              <name > 100 </name >
              </search_scores >
              </item >
              <item >
              <id > 4719688 </id >
              <name > George </name >
              <ipi />
              <type > Band </type >
              <url />
              <firstname />
              <lastname />
              <update_date > 2019 - 01 - 10 14:57:44.375851 </update_date >
                     <creation_date > 2019 - 01 - 10 14:57:44.375851 </creation_date >
                            <search_scores >
                            <name > 100 </name >
                            </search_scores >
                            </item >
                            <item >
                            <id > 4719690 </id >
                            <name > George </name >
                            <ipi />
                            <type > Band </type >
                            <url />
                            <firstname />
                            <lastname />
                            <update_date > 2019 - 01 - 10 15:08:10.458386 </update_date >
                                   <creation_date > 2019 - 01 - 10 15:08:10.458386 </creation_date >
                                          <search_scores >
                                          <name > 100 </name >
                                          </search_scores >
                                          </item >
                                          <item >
                                          <id > 4481076 </id >
                                          <name > George M </name >
                                             <ipi />
                                             <type > Person </type >
                                             <url > http://data.music-story.com/george-m</url>
<firstname />
<lastname />
<update_date > 2015 - 10 - 06 09:26:30 </update_date >
       <creation_date > 2012 - 05 - 21 00:00:00 </creation_date >
              <search_scores >
              <name > 75 </name >
              </search_scores >
              </item >
              <item >
              <id > 4676313 </id >
              <name > St George </name >
                 <ipi />
                 <type > Band </type >
                 <url />
                 <firstname />
                 <lastname />
                 <update_date > 2016 - 05 - 04 16:23:54 </update_date >
                        <creation_date > 2016 - 04 - 29 17:54:25 </creation_date >
                               <search_scores >
                               <name > 66.67 </name >
                               </search_scores >
                               </item >
                               <item >
                               <id > 4705200 </id >
                               <name > George Li </name >
                                  <ipi />
                                  <type > Person </type >
                                  <url />
                                  <firstname />
                                  <lastname />
                                  <update_date > 2017 - 10 - 02 16:50:06 </update_date >
                                         <creation_date > 2017 - 10 - 02 16:50:06 </creation_date >
                                                <search_scores >
                                                <name > 66.67 </name >
                                                </search_scores >
                                                </item >
                                                <item >
                                                <id > 6264 </id >
                                                <name > Boy George </name >
                                                   <ipi />
                                                   <type > Person </type >
                                                   <url > http://data.music-story.com/boy-george</url>
<firstname > Boy George </firstname >
   <lastname > O'Dowd</lastname>
      <update_date > 2019 - 04 - 26 10:32:32.17505 </update_date >
             <creation_date > 2012 - 10 - 16 00:00:00 </creation_date >
                    <search_scores >
                    <name > 60 </name >
                    </search_scores >
                    </item >
                    <item >
                    <id > 103270 </id >
                    <name > George Kuo </name >
                       <ipi />
                       <type > Person </type >
                       <url > http://data.music-story.com/george-kuo</url>
<firstname > George </firstname >
<lastname > Kuo </lastname >
<update_date > 2015 - 10 - 06 09:26:30 </update_date >
       <creation_date > 2012 - 05 - 12 00:00:00 </creation_date >
              <search_scores >
              <name > 60 </name >
              </search_scores >
              </item >
              <item >
              <id > 110844 </id >
              <name > George Fox </name >
                 <ipi />
                 <type > Person </type >
                 <url > http://data.music-story.com/george-fox</url>
<firstname />
<lastname />
<update_date > 2015 - 10 - 06 09:26:30 </update_date >
       <creation_date > 2010 - 08 - 31 00:00:00 </creation_date >
              <search_scores >
              <name > 60 </name >
              </search_scores >
              </item >
              <item >
              <id > 111184 </id >
              <name > George Lam </name >
                 <ipi />
                 <type > Person </type >
                 <url > http://data.music-story.com/george-lam</url>
<firstname > George </firstname >
<lastname > Lam </lastname >
<update_date > 2017 - 06 - 07 11:08:58 </update_date >
       <creation_date > 2012 - 05 - 12 00:00:00 </creation_date >
              <search_scores >
              <name > 60 </name >
              </search_scores >
              </item >
              </data >
              </root > ";

            var encoding = new UTF8Encoding();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ArtistsSummary));
            ArtistsSummary artistsSummary;
            using (var s = new MemoryStream(encoding.GetBytes(source)))
            {
                artistsSummary = xmlSerializer.Deserialize(s) as ArtistsSummary;
            }

            Assert.IsNotNull(artistsSummary);
        }
    }
}
