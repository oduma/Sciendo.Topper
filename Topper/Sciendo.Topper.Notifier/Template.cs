using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Notifier
{
    public static class Template
    {
        public static class Html
        {
            public static string TodayItemsTable =>
                "<table class='{0}'><tr><th>Artist</th><th>Hits</th><th>Score</th></tr>{1}</table>";

            public static string TodayItemRow => "<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>";
            public static string TodayItemsTitle => "<h1>Today, {0}/{1}/{2}, situation is:</h1>";
            public static string NoItemsFor7Days => "No items listened at all in the last 7 days.</br>";

            public static string YearItemsTitle => "<h2>Since the beginning of the year:</h2>";

            public static string YearItemsTable =
                "<table><tr><th>Position</th><th>Artist</th><th>Score</th><th>Loved</td></tr>{0}</table>";

            public static string YearItemRow => "<tr{0}><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>";
            public static string NoItemsForYear => "Happy New Year to you.</br>";
        }

        public static string Subject => "Your Daily Music Report";
    }

}
