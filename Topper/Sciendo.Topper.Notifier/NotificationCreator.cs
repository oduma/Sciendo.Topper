using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Notifier
{
    public class NotificationCreator
    {
        private readonly IEmailSender _mailSender;

        private const string subject = "Your Daily Music Report";

        public NotificationCreator(IEmailSender mailSender)
        {
            _mailSender = mailSender;
        }

        public void ComposeAndSendMessage(IEnumerable<TopItemWithScore> todayItems,IEnumerable<TopItemWithScore> yearAggregateItems,string sendTo)
        {
            _mailSender.SendEmail(sendTo,subject,ComposeMessage(todayItems,yearAggregateItems));
        }

        private string ComposeMessage(IEnumerable<TopItemWithScore> todayItems, IEnumerable<TopItemWithScore> yearAggregateItems)
        {
            DateTime date = DateTime.Today;
            StringBuilder message =  new StringBuilder($"{Style.Definition}<h1>Today, {date.Day}/{date.Month}/{date.Year}, situation is:</h1>");
            
            if (todayItems!=null && todayItems.Any())
            {
                var todaysItemsFormatted = "<table class='{0}'><tr><th>Artist</th><th>Hits</th><th>Score</th></tr>{1}</table>";
                var rows = new StringBuilder("");
                foreach (var todayItem in todayItems)
                {
                    var row = $"<tr><td>{todayItem.Name}</td><td>{todayItem.Hits}</td><td>{todayItem.Score}</td></tr>";
                    rows.Append(row);
                }

                message.Append(string.Format(todaysItemsFormatted,Style.Today,rows.ToString()));

            }
            else
                message.Append("No items listened at all in the last 7 days.</br>");

            if (yearAggregateItems!= null && yearAggregateItems.Any())
            {
                message.Append("<h2>Since the beginning of the year:</h2>");
                var yearAggregateItemsFromatted = "<table><tr><th>Position</th><th>Artist</th><th>Score</th></tr>{0}</table>";
                var rows = new StringBuilder("");
                int position = 1;
                foreach (var yearAggregatedItem in yearAggregateItems)
                {
                    var calculatedStyle = "";
                    if (todayItems.Any((t) => t.Name == yearAggregatedItem.Name))
                        calculatedStyle = Style.Current;
                    switch (position)
                    {
                        case 1:
                            calculatedStyle += $" {Style.First}";
                            break;
                        case 2:
                            calculatedStyle += $" {Style.Second}";
                            break;
                        case 3:
                            calculatedStyle += $" {Style.Third}";
                            break;
                    }

                    if (!string.IsNullOrEmpty(calculatedStyle))
                        calculatedStyle = $" class='{calculatedStyle}'";
                    var row =
                        $"<tr{calculatedStyle}><td>{position++}</td><td>{yearAggregatedItem.Name}</td><td>{yearAggregatedItem.Score}</td></tr>";

                    rows.Append(row);
                }

                message.Append(string.Format(yearAggregateItemsFromatted, rows.ToString()));
            }
            else
                message.Append("Happy New Year to you.</br>");

            return message.ToString();

        }
    }
}
