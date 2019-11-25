﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sciendo.Topper.Domain;
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
        public void MapTopItemsToBaseEntry_Ok()
        {
            var mapTopItemToBaseEntry = new MapTopItemToBaseEntry();
            var baseEntry = mapTopItemToBaseEntry.Map(new TopItem { Name = "abc" });

            Assert.AreEqual("abc", baseEntry.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MapTopItemToBaseEntry_Null_TopItem()
        {
            var mapTopItemToBaseEntry = new MapTopItemToBaseEntry();
            var baseEntry = mapTopItemToBaseEntry.Map(null);

        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MapTopItemToBaseEntry_Null_Name()
        {
            var mapTopItemToBaseEntry = new MapTopItemToBaseEntry();
            var baseEntry = mapTopItemToBaseEntry.Map(new TopItem());

        }

        [TestMethod]
        public void MapTopItemToPosition_Ok()
        {
            var mapTopItemToPosition = new MapTopItemToPosition();
            var position = mapTopItemToPosition.Map(new TopItem { DayRanking = 1, Hits = 12, Loved = 1, Score = 44 });
            Assert.AreEqual(1, position.PositionRank);
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
            var overallEntry = mapTopItemToOverallEntry.Map(new TopItem { Name = "abc", DayRanking = 1, Hits = 12, Loved = 1, Score = 44 });
            Assert.AreEqual("abc", overallEntry.Name);
            Assert.AreEqual(1, overallEntry.Position.PositionRank);
            Assert.AreEqual(12, overallEntry.Position.Hits);
            Assert.AreEqual(1, overallEntry.Position.NoOfLovedTracks);
            Assert.AreEqual(44, overallEntry.Position.Score);
        }
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MapTopItemTOverallEntry_PositionMapper_Null()
        {
            var mapTopItemToOverallEntry = new MapTopItemToOverallEntry(null);
            var overallEntry = mapTopItemToOverallEntry.Map(new TopItem { Name = "abc", DayRanking = 1, Hits = 12, Loved = 1, Score = 44 });
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
                (new TopItem { Name = "abc", DayRanking = 1, Hits = 12, Loved = 1, Score = 44 },new TopItem { Name = "abc", DayRanking = 2, Hits = 10, Loved = 1, Score = 30 });
            Assert.AreEqual("abc", overallEntreyEvolyion.Name);
            Assert.AreEqual(1, overallEntreyEvolyion.Position.PositionRank);
            Assert.AreEqual(12, overallEntreyEvolyion.Position.Hits);
            Assert.AreEqual(1, overallEntreyEvolyion.Position.NoOfLovedTracks);
            Assert.AreEqual(44, overallEntreyEvolyion.Position.Score);
            Assert.AreEqual(2, overallEntreyEvolyion.PreviousDayOverallPosition.PositionRank);
            Assert.AreEqual(10, overallEntreyEvolyion.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(1, overallEntreyEvolyion.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(30, overallEntreyEvolyion.PreviousDayOverallPosition.Score);
        }
        [TestMethod]
        public void MapTopItemTOverallEntryEvolution_Evolution_NewEntry()
        {
            var mapTopItemToOverallEntryEvolution = new MapTopItemToOverallEntryEvolution(new MapTopItemToPosition());
            var overallEntreyEvolyion = mapTopItemToOverallEntryEvolution.Map
                (new TopItem { Name = "abc", DayRanking = 1, Hits = 12, Loved = 1, Score = 44 }, null);
            Assert.AreEqual("abc", overallEntreyEvolyion.Name);
            Assert.AreEqual(1, overallEntreyEvolyion.Position.PositionRank);
            Assert.AreEqual(12, overallEntreyEvolyion.Position.Hits);
            Assert.AreEqual(1, overallEntreyEvolyion.Position.NoOfLovedTracks);
            Assert.AreEqual(44, overallEntreyEvolyion.Position.Score);
            Assert.IsNull(overallEntreyEvolyion.PreviousDayOverallPosition);
        }
        [TestMethod]
        public void MapTopItemTOverallEntryEvolution_Evolution_Exit()
        {
            var mapTopItemToOverallEntryEvolution = new MapTopItemToOverallEntryEvolution(new MapTopItemToPosition());
            var overallEntreyEvolyion = mapTopItemToOverallEntryEvolution.Map
                (null, new TopItem { Name = "abc", DayRanking = 2, Hits = 10, Loved = 1, Score = 30 });
            Assert.AreEqual("abc", overallEntreyEvolyion.Name);
            Assert.IsNull(overallEntreyEvolyion.Position);
            Assert.AreEqual(2, overallEntreyEvolyion.PreviousDayOverallPosition.PositionRank);
            Assert.AreEqual(10, overallEntreyEvolyion.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(1, overallEntreyEvolyion.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(30, overallEntreyEvolyion.PreviousDayOverallPosition.Score);
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
                (new TopItem { Name = "abc", DayRanking = 1, Hits = 12, Loved = 1, Score = 44 }, 
                new TopItem { Name = "abcd", DayRanking = 2, Hits = 10, Loved = 1, Score = 30 });
        }

        [TestMethod]
        public void MapTopItemsToOverallEntriesEvolution_AllMatched()
        {
            var mapTopItemsToOverallEntriesEvolution = new MapTopItemsToOverallEntriesEvolution(new MapTopItemToDayEntryEvolution(new MapTopItemToPosition()));
            var overalEntriesEvolution = mapTopItemsToOverallEntriesEvolution.Map(
                new[] {
                new TopItem { Name = "abc", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                new TopItem { Name = "abcd", DayRanking = 1, Hits = 14, Loved = 2, Score = 56 }},
                new[] {
                new TopItem { Name = "abc", DayRanking = 1, Hits = 12, Loved = 1, Score = 44 },
                new TopItem { Name = "abcd", DayRanking = 2, Hits = 10, Loved = 1, Score = 30 }});

            Assert.AreEqual(2, overalEntriesEvolution.Count());
            var abcEntry = overalEntriesEvolution.FirstOrDefault(o => o.Name == "abc");
            Assert.IsNotNull(abcEntry);
            Assert.AreEqual("abc", abcEntry.Name);
            Assert.AreEqual(1, abcEntry.Position.PositionRank);
            Assert.AreEqual(1, abcEntry.PreviousDayOverallPosition.PositionRank);
            Assert.AreEqual(14, abcEntry.Position.Hits);
            Assert.AreEqual(12, abcEntry.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(1, abcEntry.Position.NoOfLovedTracks);
            Assert.AreEqual(1, abcEntry.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(70, abcEntry.Position.Score);
            Assert.AreEqual(44, abcEntry.PreviousDayOverallPosition.Score);
            var abcdEntry = overalEntriesEvolution.FirstOrDefault(o => o.Name == "abcd");
            Assert.IsNotNull(abcEntry);
            Assert.AreEqual("abcd", abcdEntry.Name);
            Assert.AreEqual(1, abcdEntry.Position.PositionRank);
            Assert.AreEqual(2, abcdEntry.PreviousDayOverallPosition.PositionRank);
            Assert.AreEqual(14, abcdEntry.Position.Hits);
            Assert.AreEqual(10, abcdEntry.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(2, abcdEntry.Position.NoOfLovedTracks);
            Assert.AreEqual(1, abcdEntry.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(56, abcdEntry.Position.Score);
            Assert.AreEqual(30, abcdEntry.PreviousDayOverallPosition.Score);
        }

        [TestMethod]
        public void MapTopItemsToOverallEntriesEvolution_NewEntry()
        {
            var mapTopItemsToOverallEntriesEvolution = new MapTopItemsToOverallEntriesEvolution(new MapTopItemToDayEntryEvolution(new MapTopItemToPosition()));
            var overalEntriesEvolution = mapTopItemsToOverallEntriesEvolution.Map(
                new[] {
                new TopItem { Name = "abc", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                new TopItem { Name = "abcd", DayRanking = 1, Hits = 14, Loved = 2, Score = 56 }},
                new[] {
                new TopItem { Name = "abc", DayRanking = 1, Hits = 12, Loved = 1, Score = 44 }});

            Assert.AreEqual(2, overalEntriesEvolution.Count());
            var abcEntry = overalEntriesEvolution.FirstOrDefault(o => o.Name == "abc");
            Assert.IsNotNull(abcEntry);
            Assert.AreEqual("abc", abcEntry.Name);
            Assert.AreEqual(1, abcEntry.Position.PositionRank);
            Assert.AreEqual(1, abcEntry.PreviousDayOverallPosition.PositionRank);
            Assert.AreEqual(14, abcEntry.Position.Hits);
            Assert.AreEqual(12, abcEntry.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(1, abcEntry.Position.NoOfLovedTracks);
            Assert.AreEqual(1, abcEntry.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(70, abcEntry.Position.Score);
            Assert.AreEqual(44, abcEntry.PreviousDayOverallPosition.Score);
            var abcdEntry = overalEntriesEvolution.FirstOrDefault(o => o.Name == "abcd");
            Assert.IsNotNull(abcEntry);
            Assert.AreEqual("abcd", abcdEntry.Name);
            Assert.AreEqual(1, abcdEntry.Position.PositionRank);
            Assert.IsNull(abcdEntry.PreviousDayOverallPosition);
            Assert.AreEqual(14, abcdEntry.Position.Hits);
            Assert.AreEqual(2, abcdEntry.Position.NoOfLovedTracks);
            Assert.AreEqual(56, abcdEntry.Position.Score);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MapTopItemsToOverallEntriesEvolution_NoOverallMapper()
        {
            var mapTopItemsToOverallEntriesEvolution = new MapTopItemsToOverallEntriesEvolution(null);
            var overalEntriesEvolution = mapTopItemsToOverallEntriesEvolution.Map(
                new[] {
                new TopItem { Name = "abc", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                new TopItem { Name = "abcd", DayRanking = 1, Hits = 14, Loved = 2, Score = 56 }},
                new[] {
                new TopItem { Name = "abc", DayRanking = 1, Hits = 12, Loved = 1, Score = 44 },
                new TopItem { Name = "abcd", DayRanking = 2, Hits = 10, Loved = 1, Score = 30 }}).ToArray();
        }

        [TestMethod]
        public void MapTopItemToDayEntryEvolution_Ok()
        {
            var mapTopItemToDayEntryEvolution = new MapTopItemToDayEntryEvolution(new MapTopItemToPosition());
            var dayEntryEvolution = mapTopItemToDayEntryEvolution.Map(
                new TopItem { Name = "abc", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                new TopItem { Name = "abc", DayRanking = 25, Hits = 140, Loved = 3, Score = 700 },
                new TopItem { Name = "abc", DayRanking = 2, Hits = 10, Loved = 0, Score = 44 },
                new TopItem { Name = "abc", DayRanking = 35, Hits = 130, Loved = 2, Score = 680 });
            Assert.AreEqual("abc", dayEntryEvolution.Name);
            Assert.AreEqual(1, dayEntryEvolution.CurrentDayPosition.PositionRank);
            Assert.AreEqual(14, dayEntryEvolution.CurrentDayPosition.Hits);
            Assert.AreEqual(1, dayEntryEvolution.CurrentDayPosition.NoOfLovedTracks);
            Assert.AreEqual(70, dayEntryEvolution.CurrentDayPosition.Score);
            Assert.AreEqual(2, dayEntryEvolution.PreviousDayPosition.PositionRank);
            Assert.AreEqual(10, dayEntryEvolution.PreviousDayPosition.Hits);
            Assert.AreEqual(0, dayEntryEvolution.PreviousDayPosition.NoOfLovedTracks);
            Assert.AreEqual(44, dayEntryEvolution.PreviousDayPosition.Score);
            Assert.AreEqual(25, dayEntryEvolution.Position.PositionRank);
            Assert.AreEqual(140, dayEntryEvolution.Position.Hits);
            Assert.AreEqual(3, dayEntryEvolution.Position.NoOfLovedTracks);
            Assert.AreEqual(700, dayEntryEvolution.Position.Score);
            Assert.AreEqual(35, dayEntryEvolution.PreviousDayOverallPosition.PositionRank);
            Assert.AreEqual(130, dayEntryEvolution.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(2, dayEntryEvolution.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(680, dayEntryEvolution.PreviousDayOverallPosition.Score);
        }

        [TestMethod]
        public void MapTopItemToDayEntryEvolution_NewEntry()
        {
            var mapTopItemToDayEntryEvolution = new MapTopItemToDayEntryEvolution(new MapTopItemToPosition());
            var dayEntryEvolution = mapTopItemToDayEntryEvolution.Map(
                new TopItem { Name = "abc", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                new TopItem { Name = "abc", DayRanking = 25, Hits = 140, Loved = 3, Score = 700 },
                null,
                null);
            Assert.AreEqual("abc", dayEntryEvolution.Name);
            Assert.AreEqual(1, dayEntryEvolution.CurrentDayPosition.PositionRank);
            Assert.AreEqual(14, dayEntryEvolution.CurrentDayPosition.Hits);
            Assert.AreEqual(1, dayEntryEvolution.CurrentDayPosition.NoOfLovedTracks);
            Assert.AreEqual(70, dayEntryEvolution.CurrentDayPosition.Score);
            Assert.IsNull(dayEntryEvolution.PreviousDayPosition);
            Assert.AreEqual(25, dayEntryEvolution.Position.PositionRank);
            Assert.AreEqual(140, dayEntryEvolution.Position.Hits);
            Assert.AreEqual(3, dayEntryEvolution.Position.NoOfLovedTracks);
            Assert.AreEqual(700, dayEntryEvolution.Position.Score);
            Assert.IsNull(dayEntryEvolution.PreviousDayOverallPosition);
        }

        [TestMethod]
        public void MapTopItemToDayEntryEvolution_ReEntry()
        {
            var mapTopItemToDayEntryEvolution = new MapTopItemToDayEntryEvolution(new MapTopItemToPosition());
            var dayEntryEvolution = mapTopItemToDayEntryEvolution.Map(
                new TopItem { Name = "abc", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                new TopItem { Name = "abc", DayRanking = 25, Hits = 140, Loved = 3, Score = 700 },
                null,
                new TopItem { Name = "abc", DayRanking = 35, Hits = 130, Loved = 2, Score = 680 });
            Assert.AreEqual("abc", dayEntryEvolution.Name);
            Assert.AreEqual(1, dayEntryEvolution.CurrentDayPosition.PositionRank);
            Assert.AreEqual(14, dayEntryEvolution.CurrentDayPosition.Hits);
            Assert.AreEqual(1, dayEntryEvolution.CurrentDayPosition.NoOfLovedTracks);
            Assert.AreEqual(70, dayEntryEvolution.CurrentDayPosition.Score);
            Assert.IsNull(dayEntryEvolution.PreviousDayPosition);
            Assert.AreEqual(25, dayEntryEvolution.Position.PositionRank);
            Assert.AreEqual(140, dayEntryEvolution.Position.Hits);
            Assert.AreEqual(3, dayEntryEvolution.Position.NoOfLovedTracks);
            Assert.AreEqual(700, dayEntryEvolution.Position.Score);
            Assert.AreEqual(35, dayEntryEvolution.PreviousDayOverallPosition.PositionRank);
            Assert.AreEqual(130, dayEntryEvolution.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(2, dayEntryEvolution.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(680, dayEntryEvolution.PreviousDayOverallPosition.Score);
        }

        [TestMethod]
        public void MapTopItemToDayEntryEvolution_Exit()
        {
            var mapTopItemToDayEntryEvolution = new MapTopItemToDayEntryEvolution(new MapTopItemToPosition());
            var dayEntryEvolution = mapTopItemToDayEntryEvolution.Map(null,
                new TopItem { Name = "abc", DayRanking = 25, Hits = 140, Loved = 3, Score = 700 },
                new TopItem { Name = "abc", DayRanking = 2, Hits = 10, Loved = 0, Score = 44 },
                new TopItem { Name = "abc", DayRanking = 35, Hits = 130, Loved = 2, Score = 680 });
            Assert.AreEqual("abc", dayEntryEvolution.Name);
            Assert.IsNull(dayEntryEvolution.CurrentDayPosition);
            Assert.AreEqual(2, dayEntryEvolution.PreviousDayPosition.PositionRank);
            Assert.AreEqual(10, dayEntryEvolution.PreviousDayPosition.Hits);
            Assert.AreEqual(0, dayEntryEvolution.PreviousDayPosition.NoOfLovedTracks);
            Assert.AreEqual(44, dayEntryEvolution.PreviousDayPosition.Score);
            Assert.AreEqual(25, dayEntryEvolution.Position.PositionRank);
            Assert.AreEqual(140, dayEntryEvolution.Position.Hits);
            Assert.AreEqual(3, dayEntryEvolution.Position.NoOfLovedTracks);
            Assert.AreEqual(700, dayEntryEvolution.Position.Score);
            Assert.AreEqual(35, dayEntryEvolution.PreviousDayOverallPosition.PositionRank);
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
                new TopItem { Name = "abc", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                new TopItem { Name = "abc", DayRanking = 25, Hits = 140, Loved = 3, Score = 700 },
                new TopItem { Name = "abcd", DayRanking = 2, Hits = 10, Loved = 0, Score = 44 },
                new TopItem { Name = "abc", DayRanking = 35, Hits = 130, Loved = 2, Score = 680 });
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MapTopItemToDayEntryEvolution_UnMatched_Different_Level()
        {
            var mapTopItemToDayEntryEvolution = new MapTopItemToDayEntryEvolution(new MapTopItemToPosition());
            var dayEntryEvolution = mapTopItemToDayEntryEvolution.Map(
                new TopItem { Name = "abc", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                new TopItem { Name = "abcd", DayRanking = 25, Hits = 140, Loved = 3, Score = 700 },
                new TopItem { Name = "abc", DayRanking = 2, Hits = 10, Loved = 0, Score = 44 },
                new TopItem { Name = "abcd", DayRanking = 35, Hits = 130, Loved = 2, Score = 680 });
        }

        [TestMethod]
        public void MapTopItemsToDayEntriesEvolution_Ok() 
        {
            var mapTopItemsToDayEntriesEvolution = new MapTopItemsToDayEntriesEvolution(new MapTopItemToDayEntryEvolution(new MapTopItemToPosition()));
            var dayEntriesEvolution = mapTopItemsToDayEntriesEvolution.Map(
                new[] 
                {
                    new TopItem { Name = "abc", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                    new TopItem { Name = "abcd", DayRanking = 2, Hits = 10, Loved = 0, Score = 44 }
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
            Assert.AreEqual(2, abcdEntry.CurrentDayPosition.PositionRank);
            Assert.AreEqual(10, abcdEntry.CurrentDayPosition.Hits);
            Assert.AreEqual(0, abcdEntry.CurrentDayPosition.NoOfLovedTracks);
            Assert.AreEqual(44, abcdEntry.CurrentDayPosition.Score);
            Assert.AreEqual(1, abcdEntry.PreviousDayPosition.PositionRank);
            Assert.AreEqual(10, abcdEntry.PreviousDayPosition.Hits);
            Assert.AreEqual(0, abcdEntry.PreviousDayPosition.NoOfLovedTracks);
            Assert.AreEqual(44, abcdEntry.PreviousDayPosition.Score);
            Assert.AreEqual(35, abcdEntry.Position.PositionRank);
            Assert.AreEqual(130, abcdEntry.Position.Hits);
            Assert.AreEqual(2, abcdEntry.Position.NoOfLovedTracks);
            Assert.AreEqual(680, abcdEntry.Position.Score);
            Assert.AreEqual(38, abcdEntry.PreviousDayOverallPosition.PositionRank);
            Assert.AreEqual(110, abcdEntry.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(2, abcdEntry.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(580, abcdEntry.PreviousDayOverallPosition.Score);

            var abcEntry = dayEntriesEvolution.FirstOrDefault(d => d.Name == "abc");
            Assert.IsNotNull(abcEntry);
            Assert.AreEqual("abc", abcEntry.Name);
            Assert.AreEqual(1, abcEntry.CurrentDayPosition.PositionRank);
            Assert.AreEqual(14, abcEntry.CurrentDayPosition.Hits);
            Assert.AreEqual(1, abcEntry.CurrentDayPosition.NoOfLovedTracks);
            Assert.AreEqual(70, abcEntry.CurrentDayPosition.Score);
            Assert.AreEqual(2, abcEntry.PreviousDayPosition.PositionRank);
            Assert.AreEqual(9, abcEntry.PreviousDayPosition.Hits);
            Assert.AreEqual(1, abcEntry.PreviousDayPosition.NoOfLovedTracks);
            Assert.AreEqual(34, abcEntry.PreviousDayPosition.Score);
            Assert.AreEqual(25, abcEntry.Position.PositionRank);
            Assert.AreEqual(140, abcEntry.Position.Hits);
            Assert.AreEqual(3, abcEntry.Position.NoOfLovedTracks);
            Assert.AreEqual(700, abcEntry.Position.Score);
            Assert.AreEqual(35, abcEntry.PreviousDayOverallPosition.PositionRank);
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
                    new TopItem { Name = "abc", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 },
                    new TopItem { Name = "abcd", DayRanking = 2, Hits = 10, Loved = 0, Score = 44 }
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
            Assert.AreEqual(2, abcdEntry.CurrentDayPosition.PositionRank);
            Assert.AreEqual(10, abcdEntry.CurrentDayPosition.Hits);
            Assert.AreEqual(0, abcdEntry.CurrentDayPosition.NoOfLovedTracks);
            Assert.AreEqual(44, abcdEntry.CurrentDayPosition.Score);
            Assert.AreEqual(1, abcdEntry.PreviousDayPosition.PositionRank);
            Assert.AreEqual(10, abcdEntry.PreviousDayPosition.Hits);
            Assert.AreEqual(0, abcdEntry.PreviousDayPosition.NoOfLovedTracks);
            Assert.AreEqual(44, abcdEntry.PreviousDayPosition.Score);
            Assert.AreEqual(35, abcdEntry.Position.PositionRank);
            Assert.AreEqual(130, abcdEntry.Position.Hits);
            Assert.AreEqual(2, abcdEntry.Position.NoOfLovedTracks);
            Assert.AreEqual(680, abcdEntry.Position.Score);
            Assert.AreEqual(38, abcdEntry.PreviousDayOverallPosition.PositionRank);
            Assert.AreEqual(110, abcdEntry.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(2, abcdEntry.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(580, abcdEntry.PreviousDayOverallPosition.Score);

            var abcEntry = dayEntriesEvolution.FirstOrDefault(d => d.Name == "abc");
            Assert.IsNotNull(abcEntry);
            Assert.AreEqual("abc", abcEntry.Name);
            Assert.AreEqual(1, abcEntry.CurrentDayPosition.PositionRank);
            Assert.AreEqual(14, abcEntry.CurrentDayPosition.Hits);
            Assert.AreEqual(1, abcEntry.CurrentDayPosition.NoOfLovedTracks);
            Assert.AreEqual(70, abcEntry.CurrentDayPosition.Score);
            Assert.IsNull(abcEntry.PreviousDayPosition);
            Assert.AreEqual(25, abcEntry.Position.PositionRank);
            Assert.AreEqual(140, abcEntry.Position.Hits);
            Assert.AreEqual(3, abcEntry.Position.NoOfLovedTracks);
            Assert.AreEqual(700, abcEntry.Position.Score);
            Assert.IsNull(abcEntry.PreviousDayOverallPosition);
        }

        [TestMethod]
        public void MapTopItemsToDayEntriesEvolution_Exit()
        {
            var mapTopItemsToDayEntriesEvolution = new MapTopItemsToDayEntriesEvolution(new MapTopItemToDayEntryEvolution(new MapTopItemToPosition()));
            var dayEntriesEvolution = mapTopItemsToDayEntriesEvolution.Map(
                new[]
                {
                    new TopItem { Name = "abc", DayRanking = 1, Hits = 14, Loved = 1, Score = 70 }
                },
                new[]
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
            Assert.AreEqual(1, dayEntriesEvolution.Count());

            var abcEntry = dayEntriesEvolution.FirstOrDefault(d => d.Name == "abc");
            Assert.IsNotNull(abcEntry);
            Assert.AreEqual("abc", abcEntry.Name);
            Assert.AreEqual(1, abcEntry.CurrentDayPosition.PositionRank);
            Assert.AreEqual(14, abcEntry.CurrentDayPosition.Hits);
            Assert.AreEqual(1, abcEntry.CurrentDayPosition.NoOfLovedTracks);
            Assert.AreEqual(70, abcEntry.CurrentDayPosition.Score);
            Assert.AreEqual(2, abcEntry.PreviousDayPosition.PositionRank);
            Assert.AreEqual(9, abcEntry.PreviousDayPosition.Hits);
            Assert.AreEqual(1, abcEntry.PreviousDayPosition.NoOfLovedTracks);
            Assert.AreEqual(34, abcEntry.PreviousDayPosition.Score);
            Assert.AreEqual(25, abcEntry.Position.PositionRank);
            Assert.AreEqual(140, abcEntry.Position.Hits);
            Assert.AreEqual(3, abcEntry.Position.NoOfLovedTracks);
            Assert.AreEqual(700, abcEntry.Position.Score);
            Assert.AreEqual(35, abcEntry.PreviousDayOverallPosition.PositionRank);
            Assert.AreEqual(120, abcEntry.PreviousDayOverallPosition.Hits);
            Assert.AreEqual(3, abcEntry.PreviousDayOverallPosition.NoOfLovedTracks);
            Assert.AreEqual(600, abcEntry.PreviousDayOverallPosition.Score);
        }
    }
}