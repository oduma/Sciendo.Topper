using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sciendo.Last.Fm;
using Sciendo.Last.Fm.DataTypes;
using Sciendo.Topper.Source;
using Sciendo.Topper.Source.DataTypes;

namespace Sciendo.Topper.Tests.Unit
{
    [TestClass]
    public class TopItemsSourcerTests   
    {
        [TestMethod]
        public void NoSourcersData()
        {
            var topartistsContentProviderMock = new Mock<IContentProvider<TopArtistsRootObject>>();
            topartistsContentProviderMock.Setup((m) => m.GetContent("user.gettopartists", "scentmaster", 1, "period=7day"))
                .Returns(new TopArtistsRootObject());
            var lovedTracksContentProviderMock = new Mock<IContentProvider<LovedTracksRootObject>>();
            lovedTracksContentProviderMock.Setup((m) => m.GetContent("user.getlovedtracks", "scentmaster",1,""))
                .Returns(new LovedTracksRootObject());
            TopItemsAggregator topItemsSourcer = new TopItemsAggregator();
            topItemsSourcer.RegisterProvider(new LastFmTopArtistProvider(topartistsContentProviderMock.Object));
            topItemsSourcer.RegisterProvider(new LastFmLovedProvider(lovedTracksContentProviderMock.Object));
            var topItems = topItemsSourcer.GetItems("scentmaster");
            Assert.IsNotNull(topItems);
            Assert.IsFalse(topItems.Any());
        }

        [TestMethod]
        public void FirstSourcersDataOnly()
        {
            var topArtistsReturn = new TopArtistsRootObject
            {
                TopArtistsPage = new TopArtistsPage
                {
                    TopArtists = new TopArtist[]
                        {new TopArtist {Name = "one", PlayCount = 2}, new TopArtist {Name = "two", PlayCount = 2}}
                }
            };
            var topartistsContentProviderMock = new Mock<IContentProvider<TopArtistsRootObject>>();
            topartistsContentProviderMock.Setup((m) => m.GetContent("user.gettopartists", "scentmaster", 1, "period=7day"))
                .Returns(topArtistsReturn);
            var lovedTracksContentProviderMock = new Mock<IContentProvider<LovedTracksRootObject>>();
            lovedTracksContentProviderMock.Setup((m) => m.GetContent("user.getlovedtracks", "scentmaster", 1, ""))
                .Returns(new LovedTracksRootObject());
            TopItemsAggregator topItemsSourcer = new TopItemsAggregator();
            topItemsSourcer.RegisterProvider(new LastFmTopArtistProvider(topartistsContentProviderMock.Object));
            topItemsSourcer.RegisterProvider(new LastFmLovedProvider(lovedTracksContentProviderMock.Object));
            var topItems = topItemsSourcer.GetItems("scentmaster");
            Assert.IsNotNull(topItems);
            Assert.IsTrue(topItems.Any());
            Assert.IsTrue(topItems.Any(t=>t.Hits==2 && t.Name=="one" && t.Loved==0));
            Assert.IsTrue(topItems.Any(t => t.Hits == 2 && t.Name == "two" && t.Loved==0));
            Assert.AreEqual(2,topItems.Count);
        }
        [TestMethod]
        public void SecondSourcersDataOnly()
        {
            var lovedTracksRootReturn = new LovedTracksRootObject
            {
                LovedTracksPage = new LovedTracksPage
                {
                    LovedTracks = new LovedTrack[]
                    {
                        new LovedTrack
                        {
                            Name = "onesong", Artist = new Item {Name = "one"},
                            Date = new LastFmDate {Text = "some date", Uts = 1539280493}
                        },
                        new LovedTrack
                        {
                            Name = "twosong", Artist = new Item {Name = "one"},
                            Date = new LastFmDate {Text = "some date", Uts = 1539280493}
                        }
                    }
                }
            };
            var topartistsContentProviderMock = new Mock<IContentProvider<TopArtistsRootObject>>();
            topartistsContentProviderMock.Setup((m) => m.GetContent("user.gettopartists", "scentmaster", 1, "period=7day"))
                .Returns(new TopArtistsRootObject());
            var lovedTracksContentProviderMock = new Mock<IContentProvider<LovedTracksRootObject>>();
            lovedTracksContentProviderMock.Setup((m) => m.GetContent("user.getlovedtracks", "scentmaster", 1, ""))
                .Returns(lovedTracksRootReturn);
            TopItemsAggregator topItemsSourcer = new TopItemsAggregator();
            topItemsSourcer.RegisterProvider(new LastFmTopArtistProvider(topartistsContentProviderMock.Object));
            topItemsSourcer.RegisterProvider(new LastFmLovedProvider(lovedTracksContentProviderMock.Object));
            var topItems = topItemsSourcer.GetItems("scentmaster");
            Assert.IsNotNull(topItems);
            Assert.IsTrue(topItems.Any());
            Assert.IsFalse(topItems.Any(t => t.Hits >0));
            Assert.IsTrue(topItems.Any(t => t.Hits == 0 && t.Name == "one" && t.Loved == 2));
            Assert.AreEqual(1, topItems.Count);
        }

        [TestMethod]
        public void BothSourcersData()
        {
            var topArtistsReturn = new TopArtistsRootObject
            {
                TopArtistsPage = new TopArtistsPage
                {
                    TopArtists = new TopArtist[]
                        {new TopArtist {Name = "one", PlayCount = 2}, new TopArtist {Name = "two", PlayCount = 2}}
                }
            };

            var lovedTracksRootReturn = new LovedTracksRootObject
            {
                LovedTracksPage = new LovedTracksPage
                {
                    LovedTracks = new LovedTrack[]
                    {
                        new LovedTrack
                        {
                            Name = "onesong", Artist = new Item {Name = "one"},
                            Date = new LastFmDate {Text = "some date", Uts = 1539280493}
                        },
                        new LovedTrack
                        {
                            Name = "twosong", Artist = new Item {Name = "one"},
                            Date = new LastFmDate {Text = "some date", Uts = 1539280493}
                        }
                    }
                }
            };
            var topartistsContentProviderMock = new Mock<IContentProvider<TopArtistsRootObject>>();
            topartistsContentProviderMock.Setup((m) => m.GetContent("user.gettopartists", "scentmaster", 1, "period=7day"))
                .Returns(topArtistsReturn);
            var lovedTracksContentProviderMock = new Mock<IContentProvider<LovedTracksRootObject>>();
            lovedTracksContentProviderMock.Setup((m) => m.GetContent("user.getlovedtracks", "scentmaster", 1, ""))
                .Returns(lovedTracksRootReturn);
            TopItemsAggregator topItemsSourcer = new TopItemsAggregator();
            topItemsSourcer.RegisterProvider(new LastFmTopArtistProvider(topartistsContentProviderMock.Object));
            topItemsSourcer.RegisterProvider(new LastFmLovedProvider(lovedTracksContentProviderMock.Object));
            var topItems = topItemsSourcer.GetItems("scentmaster");
            Assert.IsNotNull(topItems);
            Assert.IsTrue(topItems.Any());
            Assert.IsTrue(topItems.Any(t => t.Hits == 2 && t.Name == "one" && t.Loved == 2));
            Assert.AreEqual(2, topItems.Count);
            Assert.IsTrue(topItems.Any(t => t.Hits == 2 && t.Name == "two" && t.Loved == 0));
        }
    }
}
