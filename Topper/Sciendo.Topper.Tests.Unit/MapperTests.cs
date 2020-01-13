﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using Sciendo.Topper.Service;
using Sciendo.Topper.Service.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sciendo.Topper.Tests.Unit
{
    [TestClass]
    public class MapperTests
    {

        [TestMethod]
        public void MapTopItemToPosition_Ok()
        {
            var mapTopItemToPosition = new MapTopItemToPosition();
            var position = mapTopItemToPosition.Map(new TopItem { DayRanking = 1, Hits = 12, Loved = 1, Score = 44 });
            Assert.AreEqual(1, position.Rank);
            Assert.AreEqual(12, position.Hits);
            Assert.AreEqual(1, position.NoOfLovedTracks);
            Assert.AreEqual(44, position.Score);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MapTopItemToPosition_Null_TopItem()
        {
            var mapTopItemToPosition = new MapTopItemToPosition();
            var position = mapTopItemToPosition.Map(null);
        }

        [TestMethod]
        public void MapTopItemTOverallEntry_Ok()
        {
            var mapTopItemToOverallEntry = new MapTopItemToOverallEntry(new MapTopItemToPosition());
            var overallEntry = mapTopItemToOverallEntry.Map(new TopItemWithPictureUrl { Name = "abc", PictureUrl = "http://myownrepo/a/abc.jpg",DayRanking = 1, Hits = 12, Loved = 1, Score = 44 });
            Assert.AreEqual("abc", overallEntry.Name);
            Assert.AreEqual("http://myownrepo/a/abc.jpg", overallEntry.PictureUrl);
            Assert.AreEqual(1, overallEntry.CurrentOverallPosition.Rank);
            Assert.AreEqual(12, overallEntry.CurrentOverallPosition.Hits);
            Assert.AreEqual(1, overallEntry.CurrentOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(44, overallEntry.CurrentOverallPosition.Score);
        }
        
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MapTopItemTOverallEntry_PositionMapper_Null()
        {
            var mapTopItemToOverallEntry = new MapTopItemToOverallEntry(null);
            var overallEntry = mapTopItemToOverallEntry.Map(new TopItemWithPictureUrl { Name = "abc", PictureUrl= "http://myownrepo/a/abc.jpg",DayRanking = 1, Hits = 12, Loved = 1, Score = 44 });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MapTopItemTOverallEntry_TopItem_Null()
        {
            var mapTopItemToOverallEntry = new MapTopItemToOverallEntry(new MapTopItemToPosition());
            var overallEntry = mapTopItemToOverallEntry.Map(null);
        }
        
        [TestMethod]
        public void MapTopItemTOverallEntryEvolution_Evolution_Positive()
        {
            var mapTopItemToOverallEntryEvolution = new MapTopItemToOverallEntryEvolution(new MapTopItemToPosition());
            var overallEntreyEvolyion = mapTopItemToOverallEntryEvolution.Map
                (new TopItemWithPictureUrl { Name = "abc", PictureUrl= "http://myownrepo/a/abc.jpg",DayRanking = 1, Hits = 12, Loved = 1, Score = 44 },new TopItem { Name = "abc", DayRanking = 2, Hits = 10, Loved = 1, Score = 30 });
            Assert.AreEqual("abc", overallEntreyEvolyion.Name);
            Assert.AreEqual("http://myownrepo/a/abc.jpg", overallEntreyEvolyion.PictureUrl);
            Assert.AreEqual(1, overallEntreyEvolyion.CurrentOverallPosition.Rank);
            Assert.AreEqual(12, overallEntreyEvolyion.CurrentOverallPosition.Hits);
            Assert.AreEqual(1, overallEntreyEvolyion.CurrentOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(44, overallEntreyEvolyion.CurrentOverallPosition.Score);
            Assert.AreEqual(2, overallEntreyEvolyion.PreviousDayOverallPosition.Rank);
            Assert.AreEqual(10, overallEntreyEvolyion.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(1, overallEntreyEvolyion.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(30, overallEntreyEvolyion.PreviousDayOverallPosition.Score);
        }
        
        [TestMethod]
        public void MapTopItemTOverallEntryEvolution_Evolution_NewEntry()
        {
            var mapTopItemToOverallEntryEvolution = new MapTopItemToOverallEntryEvolution(new MapTopItemToPosition());
            var overallEntreyEvolyion = mapTopItemToOverallEntryEvolution.Map
                (new TopItemWithPictureUrl { Name = "abc", PictureUrl= "http://myownrepo/a/abc.jpg",DayRanking = 1, Hits = 12, Loved = 1, Score = 44 }, null);
            Assert.AreEqual("abc", overallEntreyEvolyion.Name);
            Assert.AreEqual("http://myownrepo/a/abc.jpg", overallEntreyEvolyion.PictureUrl);
            Assert.AreEqual(1, overallEntreyEvolyion.CurrentOverallPosition.Rank);
            Assert.AreEqual(12, overallEntreyEvolyion.CurrentOverallPosition.Hits);
            Assert.AreEqual(1, overallEntreyEvolyion.CurrentOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(44, overallEntreyEvolyion.CurrentOverallPosition.Score);
            Assert.IsNull(overallEntreyEvolyion.PreviousDayOverallPosition);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MapTopItemTOverallEntryEvolution_Evolution_Both_TopItems_Null()
        {
            var mapTopItemToOverallEntryEvolution = new MapTopItemToOverallEntryEvolution(new MapTopItemToPosition());
            var overallEntreyEvolyion = mapTopItemToOverallEntryEvolution.Map
                (null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MapTopItemTOverallEntryEvolution_Evolution_Different()
        {

            var mapTopItemToOverallEntryEvolution = new MapTopItemToOverallEntryEvolution(new MapTopItemToPosition());
            var overallEntreyEvolyion = mapTopItemToOverallEntryEvolution.Map
                (new TopItemWithPictureUrl { Name = "abc", PictureUrl= "http://myownrepo/a/abc.jpg",DayRanking = 1, Hits = 12, Loved = 1, Score = 44 }, 
                new TopItem { Name = "abcd", DayRanking = 2, Hits = 10, Loved = 1, Score = 30 });
        }

        [TestMethod]
        public void MapTopItemsToOverallEntriesEvolution_AllMatched()
        {
            var mapTopItemsToOverallEntriesEvolution = new MapTopItemsToOverallEntriesEvolution(new MapTopItemToOverallEntryEvolution(new MapTopItemToPosition()));
            var overalEntriesEvolution = mapTopItemsToOverallEntriesEvolution.Map(
                new[] {
                new TopItemWithPictureUrl { Name = "abc", PictureUrl="http://myownrepo/a/abc.jpg", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                new TopItemWithPictureUrl { Name = "abcd", PictureUrl = "http://myownrepo/a/abcd.jpg", DayRanking = 1, Hits = 14, Loved = 2, Score = 56 }},
                new[] {
                new TopItem { Name = "abc", DayRanking = 1, Hits = 12, Loved = 1, Score = 44 },
                new TopItem { Name = "abcd", DayRanking = 2, Hits = 10, Loved = 1, Score = 30 }});

            Assert.AreEqual(2, overalEntriesEvolution.Count());
            var abcEntry = overalEntriesEvolution.FirstOrDefault(o => o.Name == "abc");
            Assert.IsNotNull(abcEntry);
            Assert.AreEqual("abc", abcEntry.Name);
            Assert.AreEqual("http://myownrepo/a/abc.jpg", abcEntry.PictureUrl);
            Assert.AreEqual(1, abcEntry.CurrentOverallPosition.Rank);
            Assert.AreEqual(1, abcEntry.PreviousDayOverallPosition.Rank);
            Assert.AreEqual(14, abcEntry.CurrentOverallPosition.Hits);
            Assert.AreEqual(12, abcEntry.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(1, abcEntry.CurrentOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(1, abcEntry.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(70, abcEntry.CurrentOverallPosition.Score);
            Assert.AreEqual(44, abcEntry.PreviousDayOverallPosition.Score);
            var abcdEntry = overalEntriesEvolution.FirstOrDefault(o => o.Name == "abcd");
            Assert.IsNotNull(abcEntry);
            Assert.AreEqual("abcd", abcdEntry.Name);
            Assert.AreEqual("http://myownrepo/a/abcd.jpg", abcdEntry.PictureUrl);
            Assert.AreEqual(1, abcdEntry.CurrentOverallPosition.Rank);
            Assert.AreEqual(2, abcdEntry.PreviousDayOverallPosition.Rank);
            Assert.AreEqual(14, abcdEntry.CurrentOverallPosition.Hits);
            Assert.AreEqual(10, abcdEntry.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(2, abcdEntry.CurrentOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(1, abcdEntry.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(56, abcdEntry.CurrentOverallPosition.Score);
            Assert.AreEqual(30, abcdEntry.PreviousDayOverallPosition.Score);
        }

        [TestMethod]
        public void MapTopItemsToOverallEntriesEvolution_NewEntry()
        {

            var mapTopItemsToOverallEntriesEvolution = new MapTopItemsToOverallEntriesEvolution(new MapTopItemToOverallEntryEvolution(new MapTopItemToPosition()));
            var overalEntriesEvolution = mapTopItemsToOverallEntriesEvolution.Map(
                new[] {
                new TopItemWithPictureUrl { Name = "abc", PictureUrl = "http://myownrepo/a/abc.jpg", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                new TopItemWithPictureUrl { Name = "abcd", PictureUrl="http://myownrepo/a/abcd.jpg", DayRanking = 1, Hits = 14, Loved = 2, Score = 56 }},
                new[] {
                new TopItem { Name = "abc", DayRanking = 1, Hits = 12, Loved = 1, Score = 44 }});

            Assert.AreEqual(2, overalEntriesEvolution.Count());
            var abcEntry = overalEntriesEvolution.FirstOrDefault(o => o.Name == "abc");
            Assert.IsNotNull(abcEntry);
            Assert.AreEqual("abc", abcEntry.Name);
            Assert.AreEqual("http://myownrepo/a/abc.jpg", abcEntry.PictureUrl);
            Assert.AreEqual(1, abcEntry.CurrentOverallPosition.Rank);
            Assert.AreEqual(1, abcEntry.PreviousDayOverallPosition.Rank);
            Assert.AreEqual(14, abcEntry.CurrentOverallPosition.Hits);
            Assert.AreEqual(12, abcEntry.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(1, abcEntry.CurrentOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(1, abcEntry.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(70, abcEntry.CurrentOverallPosition.Score);
            Assert.AreEqual(44, abcEntry.PreviousDayOverallPosition.Score);
            var abcdEntry = overalEntriesEvolution.FirstOrDefault(o => o.Name == "abcd");
            Assert.IsNotNull(abcEntry);
            Assert.AreEqual("abcd", abcdEntry.Name);
            Assert.AreEqual("http://myownrepo/a/abcd.jpg", abcdEntry.PictureUrl);
            Assert.AreEqual(1, abcdEntry.CurrentOverallPosition.Rank);
            Assert.IsNull(abcdEntry.PreviousDayOverallPosition);
            Assert.AreEqual(14, abcdEntry.CurrentOverallPosition.Hits);
            Assert.AreEqual(2, abcdEntry.CurrentOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(56, abcdEntry.CurrentOverallPosition.Score);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MapTopItemsToOverallEntriesEvolution_NoOverallMapper()
        {
            var mapTopItemsToOverallEntriesEvolution = new MapTopItemsToOverallEntriesEvolution(null);
            var overalEntriesEvolution = mapTopItemsToOverallEntriesEvolution.Map(
                new[] {
                new TopItemWithPictureUrl { Name = "abc", PictureUrl = "http://myownrepo/a/abc.jpg", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                new TopItemWithPictureUrl { Name = "abcd", PictureUrl="http://myownrepo/a/abc.jpg", DayRanking = 1, Hits = 14, Loved = 2, Score = 56 }},
                new[] {
                new TopItem { Name = "abc", DayRanking = 1, Hits = 12, Loved = 1, Score = 44 },
                new TopItem { Name = "abcd", DayRanking = 2, Hits = 10, Loved = 1, Score = 30 }}).ToArray();
        }

        [TestMethod]
        public void MapTopItemToDayEntryEvolution_Ok()
        {
            var mapTopItemToDayEntryEvolution = new MapTopItemToDayEntryEvolution(new MapTopItemToPosition());
            var dayEntryEvolution = mapTopItemToDayEntryEvolution.Map(
                new TopItemWithPictureUrl { Name = "abc", PictureUrl = "http://myownrepo/a/abc.jpg", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                new TopItemWithPictureUrl { Name = "abc", PictureUrl = "http://myownrepo/a/abc.jpg", DayRanking = 25, Hits = 140, Loved = 3, Score = 700 },
                new TopItemWithPictureUrl { Name = "abc", PictureUrl = "http://myownrepo/a/abc.jpg", DayRanking = 2, Hits = 10, Loved = 0, Score = 44 },
                new TopItemWithPictureUrl { Name = "abc", PictureUrl = "http://myownrepo/a/abc.jpg", DayRanking = 35, Hits = 130, Loved = 2, Score = 680 });
            Assert.AreEqual("abc", dayEntryEvolution.Name);
            Assert.AreEqual("http://myownrepo/a/abc.jpg", dayEntryEvolution.PictureUrl);
            Assert.AreEqual(1, dayEntryEvolution.CurrentDayPosition.Rank);
            Assert.AreEqual(14, dayEntryEvolution.CurrentDayPosition.Hits);
            Assert.AreEqual(1, dayEntryEvolution.CurrentDayPosition.NoOfLovedTracks);
            Assert.AreEqual(70, dayEntryEvolution.CurrentDayPosition.Score);
            Assert.AreEqual(2, dayEntryEvolution.PreviousDayPosition.Rank);
            Assert.AreEqual(10, dayEntryEvolution.PreviousDayPosition.Hits);
            Assert.AreEqual(0, dayEntryEvolution.PreviousDayPosition.NoOfLovedTracks);
            Assert.AreEqual(44, dayEntryEvolution.PreviousDayPosition.Score);
            Assert.AreEqual(25, dayEntryEvolution.CurrentOverallPosition.Rank);
            Assert.AreEqual(140, dayEntryEvolution.CurrentOverallPosition.Hits);
            Assert.AreEqual(3, dayEntryEvolution.CurrentOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(700, dayEntryEvolution.CurrentOverallPosition.Score);
            Assert.AreEqual(35, dayEntryEvolution.PreviousDayOverallPosition.Rank);
            Assert.AreEqual(130, dayEntryEvolution.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(2, dayEntryEvolution.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(680, dayEntryEvolution.PreviousDayOverallPosition.Score);
        }

        [TestMethod]
        public void MapTopItemToDayEntryEvolution_NewEntry()
        {
            var mapTopItemToDayEntryEvolution = new MapTopItemToDayEntryEvolution(new MapTopItemToPosition());
            var dayEntryEvolution = mapTopItemToDayEntryEvolution.Map(
                new TopItemWithPictureUrl { Name = "abc", PictureUrl = "http://myownrepo/a/abc.jpg", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                new TopItemWithPictureUrl { Name = "abc", PictureUrl = "http://myownrepo/a/abc.jpg", DayRanking = 25, Hits = 140, Loved = 3, Score = 700 },
                null,
                null);
            Assert.AreEqual("abc", dayEntryEvolution.Name);
            Assert.AreEqual("http://myownrepo/a/abc.jpg", dayEntryEvolution.PictureUrl);
            Assert.AreEqual(1, dayEntryEvolution.CurrentDayPosition.Rank);
            Assert.AreEqual(14, dayEntryEvolution.CurrentDayPosition.Hits);
            Assert.AreEqual(1, dayEntryEvolution.CurrentDayPosition.NoOfLovedTracks);
            Assert.AreEqual(70, dayEntryEvolution.CurrentDayPosition.Score);
            Assert.IsNull(dayEntryEvolution.PreviousDayPosition);
            Assert.AreEqual(25, dayEntryEvolution.CurrentOverallPosition.Rank);
            Assert.AreEqual(140, dayEntryEvolution.CurrentOverallPosition.Hits);
            Assert.AreEqual(3, dayEntryEvolution.CurrentOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(700, dayEntryEvolution.CurrentOverallPosition.Score);
            Assert.IsNull(dayEntryEvolution.PreviousDayOverallPosition);
        }

        [TestMethod]
        public void MapTopItemToDayEntryEvolution_ReEntry()
        {
            var mapTopItemToDayEntryEvolution = new MapTopItemToDayEntryEvolution(new MapTopItemToPosition());
            var dayEntryEvolution = mapTopItemToDayEntryEvolution.Map(
                new TopItemWithPictureUrl { Name = "abc", PictureUrl = "http://myownrepo/a/abc.jpg", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                new TopItemWithPictureUrl { Name = "abc", PictureUrl = "http://myownrepo/a/abc.jpg", DayRanking = 25, Hits = 140, Loved = 3, Score = 700 },
                null,
                new TopItemWithPictureUrl { Name = "abc", PictureUrl = "http://myownrepo/a/abc.jpg", DayRanking = 35, Hits = 130, Loved = 2, Score = 680 });
            Assert.AreEqual("abc", dayEntryEvolution.Name);
            Assert.AreEqual("http://myownrepo/a/abc.jpg", dayEntryEvolution.PictureUrl);
            Assert.AreEqual(1, dayEntryEvolution.CurrentDayPosition.Rank);
            Assert.AreEqual(14, dayEntryEvolution.CurrentDayPosition.Hits);
            Assert.AreEqual(1, dayEntryEvolution.CurrentDayPosition.NoOfLovedTracks);
            Assert.AreEqual(70, dayEntryEvolution.CurrentDayPosition.Score);
            Assert.IsNull(dayEntryEvolution.PreviousDayPosition);
            Assert.AreEqual(25, dayEntryEvolution.CurrentOverallPosition.Rank);
            Assert.AreEqual(140, dayEntryEvolution.CurrentOverallPosition.Hits);
            Assert.AreEqual(3, dayEntryEvolution.CurrentOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(700, dayEntryEvolution.CurrentOverallPosition.Score);
            Assert.AreEqual(35, dayEntryEvolution.PreviousDayOverallPosition.Rank);
            Assert.AreEqual(130, dayEntryEvolution.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(2, dayEntryEvolution.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(680, dayEntryEvolution.PreviousDayOverallPosition.Score);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MapTopItemToDayEntryEvolution_UnMatched_OnSame_Level()
        {
            var mapTopItemToDayEntryEvolution = new MapTopItemToDayEntryEvolution(new MapTopItemToPosition());
            var dayEntryEvolution = mapTopItemToDayEntryEvolution.Map(
                new TopItemWithPictureUrl { Name = "abc", PictureUrl = "http://myownrepo/a/abc.jpg", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                new TopItemWithPictureUrl { Name = "abc", PictureUrl = "http://myownrepo/a/abc.jpg", DayRanking = 25, Hits = 140, Loved = 3, Score = 700 },
                new TopItemWithPictureUrl { Name = "abcd", PictureUrl = "http://myownrepo/a/abcd.jpg", DayRanking = 2, Hits = 10, Loved = 0, Score = 44 },
                new TopItemWithPictureUrl { Name = "abc", PictureUrl = "http://myownrepo/a/abc.jpg", DayRanking = 35, Hits = 130, Loved = 2, Score = 680 });
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MapTopItemToDayEntryEvolution_UnMatched_Different_Level()
        {
            var mapTopItemToDayEntryEvolution = new MapTopItemToDayEntryEvolution(new MapTopItemToPosition());
            var dayEntryEvolution = mapTopItemToDayEntryEvolution.Map(
                new TopItemWithPictureUrl { Name = "abc", PictureUrl = "http://myownrepo/a/abc.jpg", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                new TopItemWithPictureUrl { Name = "abcd", PictureUrl = "http://myownrepo/a/abcd.jpg", DayRanking = 25, Hits = 140, Loved = 3, Score = 700 },
                new TopItemWithPictureUrl { Name = "abc", PictureUrl = "http://myownrepo/a/abc.jpg", DayRanking = 2, Hits = 10, Loved = 0, Score = 44 },
                new TopItemWithPictureUrl { Name = "abcd", PictureUrl = "http://myownrepo/a/abcd.jpg", DayRanking = 35, Hits = 130, Loved = 2, Score = 680 });
        }

        [TestMethod]
        public void MapTopItemsToDayEntriesEvolution_Ok() 
        {
            var mapTopItemsToDayEntriesEvolution = new MapTopItemsToDayEntriesEvolution(new MapTopItemToDayEntryEvolution(new MapTopItemToPosition()));
            var dayEntriesEvolution = mapTopItemsToDayEntriesEvolution.Map(
                new[] 
                {
                    new TopItemWithPictureUrl { Name = "abc", PictureUrl= "http://myownrepo/a/abc.jpg", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                    new TopItemWithPictureUrl { Name = "abcd", PictureUrl= "http://myownrepo/a/abcd.jpg", DayRanking = 2, Hits = 10, Loved = 0, Score = 44 }
                },
                new []
                {
                    new TopItem { Name = "abc", DayRanking = 25, Hits = 140, Loved = 3, Score = 700 },
                    new TopItem { Name = "abcd", DayRanking = 35, Hits = 130, Loved = 2, Score = 680 }  
                },
                new[]
                {
                    new TopItem { Name = "abcd", DayRanking = 1, Hits = 10, Loved = 0, Score = 44 },
                    new TopItem { Name = "abc", DayRanking = 2, Hits = 9, Loved = 1, Score = 34 }
                },
                new[]
                {
                    new TopItem { Name = "abc", DayRanking = 35, Hits = 120, Loved = 3, Score = 600 },
                    new TopItem { Name = "abcd", DayRanking = 38, Hits = 110, Loved = 2, Score = 580 }
                });
            Assert.AreEqual(2, dayEntriesEvolution.Count());
            var abcdEntry = dayEntriesEvolution.FirstOrDefault(d => d.Name == "abcd");
            Assert.IsNotNull(abcdEntry);
            Assert.AreEqual("abcd", abcdEntry.Name);
            Assert.AreEqual("http://myownrepo/a/abcd.jpg",abcdEntry.PictureUrl);
            Assert.AreEqual(2, abcdEntry.CurrentDayPosition.Rank);
            Assert.AreEqual(10, abcdEntry.CurrentDayPosition.Hits);
            Assert.AreEqual(0, abcdEntry.CurrentDayPosition.NoOfLovedTracks);
            Assert.AreEqual(44, abcdEntry.CurrentDayPosition.Score);
            Assert.AreEqual(1, abcdEntry.PreviousDayPosition.Rank);
            Assert.AreEqual(10, abcdEntry.PreviousDayPosition.Hits);
            Assert.AreEqual(0, abcdEntry.PreviousDayPosition.NoOfLovedTracks);
            Assert.AreEqual(44, abcdEntry.PreviousDayPosition.Score);
            Assert.AreEqual(35, abcdEntry.CurrentOverallPosition.Rank);
            Assert.AreEqual(130, abcdEntry.CurrentOverallPosition.Hits);
            Assert.AreEqual(2, abcdEntry.CurrentOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(680, abcdEntry.CurrentOverallPosition.Score);
            Assert.AreEqual(38, abcdEntry.PreviousDayOverallPosition.Rank);
            Assert.AreEqual(110, abcdEntry.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(2, abcdEntry.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(580, abcdEntry.PreviousDayOverallPosition.Score);

            var abcEntry = dayEntriesEvolution.FirstOrDefault(d => d.Name == "abc");
            Assert.IsNotNull(abcEntry);
            Assert.AreEqual("abc", abcEntry.Name);
            Assert.AreEqual("http://myownrepo/a/abc.jpg",abcEntry.PictureUrl);
            Assert.AreEqual(1, abcEntry.CurrentDayPosition.Rank);
            Assert.AreEqual(14, abcEntry.CurrentDayPosition.Hits);
            Assert.AreEqual(1, abcEntry.CurrentDayPosition.NoOfLovedTracks);
            Assert.AreEqual(70, abcEntry.CurrentDayPosition.Score);
            Assert.AreEqual(2, abcEntry.PreviousDayPosition.Rank);
            Assert.AreEqual(9, abcEntry.PreviousDayPosition.Hits);
            Assert.AreEqual(1, abcEntry.PreviousDayPosition.NoOfLovedTracks);
            Assert.AreEqual(34, abcEntry.PreviousDayPosition.Score);
            Assert.AreEqual(25, abcEntry.CurrentOverallPosition.Rank);
            Assert.AreEqual(140, abcEntry.CurrentOverallPosition.Hits);
            Assert.AreEqual(3, abcEntry.CurrentOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(700, abcEntry.CurrentOverallPosition.Score);
            Assert.AreEqual(35, abcEntry.PreviousDayOverallPosition.Rank);
            Assert.AreEqual(120, abcEntry.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(3, abcEntry.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(600, abcEntry.PreviousDayOverallPosition.Score);
        }

        [TestMethod]
        public void MapTopItemsToDayEntriesEvolution_NewEntry()
        {
            var mapTopItemsToDayEntriesEvolution = new MapTopItemsToDayEntriesEvolution(new MapTopItemToDayEntryEvolution(new MapTopItemToPosition()));
            var dayEntriesEvolution = mapTopItemsToDayEntriesEvolution.Map(
                new[]
                {
                    new TopItemWithPictureUrl { Name = "abc", PictureUrl= "http://myownrepo/a/abc.jpg", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                    new TopItemWithPictureUrl { Name = "abcd", PictureUrl= "http://myownrepo/a/abcd.jpg", DayRanking = 2, Hits = 10, Loved = 0, Score = 44 }
                },
                new[]
                {
                    new TopItem { Name = "abc", DayRanking = 25, Hits = 140, Loved = 3, Score = 700 },
                    new TopItem { Name = "abcd", DayRanking = 35, Hits = 130, Loved = 2, Score = 680 }
                },
                new[]
                {
                    new TopItem { Name = "abcd", DayRanking = 1, Hits = 10, Loved = 0, Score = 44 }
                },
                new[]
                {
                    new TopItem { Name = "abcd", DayRanking = 38, Hits = 110, Loved = 2, Score = 580 }
                });
            Assert.AreEqual(2, dayEntriesEvolution.Count());
            var abcdEntry = dayEntriesEvolution.FirstOrDefault(d => d.Name == "abcd");
            Assert.IsNotNull(abcdEntry);
            Assert.AreEqual("abcd", abcdEntry.Name);
            Assert.AreEqual("http://myownrepo/a/abcd.jpg",abcdEntry.PictureUrl);
            Assert.AreEqual(2, abcdEntry.CurrentDayPosition.Rank);
            Assert.AreEqual(10, abcdEntry.CurrentDayPosition.Hits);
            Assert.AreEqual(0, abcdEntry.CurrentDayPosition.NoOfLovedTracks);
            Assert.AreEqual(44, abcdEntry.CurrentDayPosition.Score);
            Assert.AreEqual(1, abcdEntry.PreviousDayPosition.Rank);
            Assert.AreEqual(10, abcdEntry.PreviousDayPosition.Hits);
            Assert.AreEqual(0, abcdEntry.PreviousDayPosition.NoOfLovedTracks);
            Assert.AreEqual(44, abcdEntry.PreviousDayPosition.Score);
            Assert.AreEqual(35, abcdEntry.CurrentOverallPosition.Rank);
            Assert.AreEqual(130, abcdEntry.CurrentOverallPosition.Hits);
            Assert.AreEqual(2, abcdEntry.CurrentOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(680, abcdEntry.CurrentOverallPosition.Score);
            Assert.AreEqual(38, abcdEntry.PreviousDayOverallPosition.Rank);
            Assert.AreEqual(110, abcdEntry.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(2, abcdEntry.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(580, abcdEntry.PreviousDayOverallPosition.Score);

            var abcEntry = dayEntriesEvolution.FirstOrDefault(d => d.Name == "abc");
            Assert.IsNotNull(abcEntry);
            Assert.AreEqual("abc", abcEntry.Name);
            Assert.AreEqual("http://myownrepo/a/abc.jpg",abcEntry.PictureUrl);
            Assert.AreEqual(1, abcEntry.CurrentDayPosition.Rank);
            Assert.AreEqual(14, abcEntry.CurrentDayPosition.Hits);
            Assert.AreEqual(1, abcEntry.CurrentDayPosition.NoOfLovedTracks);
            Assert.AreEqual(70, abcEntry.CurrentDayPosition.Score);
            Assert.IsNull(abcEntry.PreviousDayPosition);
            Assert.AreEqual(25, abcEntry.CurrentOverallPosition.Rank);
            Assert.AreEqual(140, abcEntry.CurrentOverallPosition.Hits);
            Assert.AreEqual(3, abcEntry.CurrentOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(700, abcEntry.CurrentOverallPosition.Score);
            Assert.IsNull(abcEntry.PreviousDayOverallPosition);
        }

        [TestMethod]
        public void MapTopItemToEntryTimeLine_Ok()
        {
            var mapTopItemToDatedEntry = new MapTopItemToEntryTimeLine(new MapTopItemToPosition());
            var entryTimeLine = mapTopItemToDatedEntry.Map(new[] {new TopItemWithPictureUrl { Date = DateTime.Now.Date,
                Name = "abc", PictureUrl= "http://myownrepo/a/abc.jpg", DayRanking = 1, Hits = 14, Loved = 1, Score = 70, Year=DateTime.Now.Year.ToString() },
            new TopItemWithPictureUrl { Date = DateTime.Now.Date.AddDays(-1),
                Name = "abc", PictureUrl= "http://myownrepo/a/abc.jpg", DayRanking = 2, Hits = 13, Loved = 0, Score = 60, Year=DateTime.Now.Year.ToString() }});
            Assert.AreEqual("abc", entryTimeLine.Name);
            Assert.AreEqual("http://myownrepo/a/abc.jpg",entryTimeLine.PictureUrl);
            Assert.AreEqual(2, entryTimeLine.PositionAtDates.Length);
            Assert.IsTrue(entryTimeLine.PositionAtDates.Any(p => p.Date == DateTime.Now.Date.ToString("yyyy-MM-dd")));
            Assert.IsTrue(entryTimeLine.PositionAtDates.Any(p => p.Date == DateTime.Now.Date.AddDays(-1).ToString("yyy-MM-dd")));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MapTopItemToEntryTimeLine_TopItem_Null()
        {
            var mapTopItemToDatedEntry = new MapTopItemToEntryTimeLine(new MapTopItemToPosition());
            var entryTimeLine = mapTopItemToDatedEntry.Map(null);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MapTopItemToEntryTimeLine_MapperToPosition_Null()
        {

            var mapTopItemToDatedEntry = new MapTopItemToEntryTimeLine(null);
            var entryTimeLine = mapTopItemToDatedEntry.Map(new[] {new TopItemWithPictureUrl { Date = DateTime.Now.Date,
                Name = "abc", DayRanking = 1, Hits = 14, Loved = 1, Score = 70, Year=DateTime.Now.Year.ToString() },
            new TopItemWithPictureUrl { Date = DateTime.Now.Date.AddDays(-1),
                Name = "abc", DayRanking = 2, Hits = 13, Loved = 0, Score = 60, Year=DateTime.Now.Year.ToString() }});
        }

        [TestMethod]
        public void MapTopItemsToEntryTimeLines()
        {
            var mapTopItemsToEntryToTimeLines = new MapTopItemsToEntryTimeLines(new MapTopItemToEntryTimeLine(new MapTopItemToPosition()));
            var entryTimeLines = mapTopItemsToEntryToTimeLines.Map(new[]
                {
                    new TopItemWithPictureUrl { Date= DateTime.Now.AddDays(-1).Date, Name = "abc", PictureUrl= "http://myownrepo/a/abc.jpg", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                    new TopItemWithPictureUrl { Date= DateTime.Now.Date, Name = "abc", PictureUrl= "http://myownrepo/a/abc.jpg", DayRanking = 2, Hits = 10, Loved = 0, Score = 44 },
                    new TopItemWithPictureUrl { Date= DateTime.Now.AddDays(-1).Date, Name = "abcd",PictureUrl= "http://myownrepo/a/abcd.jpg", DayRanking = 3, Hits = 14, Loved = 1, Score = 70 }
                });
            Assert.AreEqual(2, entryTimeLines.Count());
            Assert.AreEqual(1, entryTimeLines.Count(d => d.Name == "abc"));
            Assert.AreEqual(1, entryTimeLines.Count(d => d.Name == "abcd"));
            Assert.AreEqual(2, entryTimeLines.FirstOrDefault(d => d.Name == "abc").PositionAtDates.Length);

        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MapTopItemsToDatedEntries_NoTopItems()
        {

            var mapTopItemsToDatedEntries = new MapTopItemsToEntryTimeLines(new MapTopItemToEntryTimeLine(new MapTopItemToPosition()));
            var datedEntries = mapTopItemsToDatedEntries.Map(null).ToArray();

        }
    }
}
