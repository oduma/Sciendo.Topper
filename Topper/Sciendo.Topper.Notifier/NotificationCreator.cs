using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Notifier
{
    public class NotificationCreator
    {
        private readonly IEmailSender _mailSender;
        private readonly string _notSendFileExtension;

        private const string subject = "Your Daily Music Report";

        public NotificationCreator(IEmailSender mailSender, string notSendFileExtension)
        {
            _mailSender = mailSender;
            _notSendFileExtension = notSendFileExtension;
        }

        public bool ComposeAndSendMessage(IEnumerable<TopItemWithScore> todayItems,IEnumerable<TopItemWithScore> yearAggregateItems,string sendTo)
        {
            var mailToBeSent = new Mail
                {To = sendTo, Subject = subject, Content = ComposeMessage(todayItems, yearAggregateItems)};
            try
            {
                _mailSender.SendEmail(mailToBeSent.To, mailToBeSent.Subject, mailToBeSent.Content);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                PersistLocallyFile(mailToBeSent);
                return false;
            }
        }

        private void PersistLocallyFile(Mail mailToBeSent)
        {
            var fileName = $"{DateTime.Now.Ticks}{mailToBeSent.To}.{_notSendFileExtension}";
            using (var fs = File.CreateText(fileName))
            {
                fs.Write(JsonConvert.SerializeObject(mailToBeSent));
            }
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
                var yearAggregateItemsFromatted = "<table><tr><th>Position</th><th>Artist</th><th>Score</th><th>Loved</td></tr>{0}</table>";
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
                        $"<tr{calculatedStyle}><td>{position++}</td><td>{yearAggregatedItem.Name}</td><td>{yearAggregatedItem.Score}</td><td>{yearAggregatedItem.Loved}</td></tr>";

                    rows.Append(row);
                }

                message.Append(string.Format(yearAggregateItemsFromatted, rows.ToString()));
            }
            else
                message.Append("Happy New Year to you.</br>");

            return message.ToString();

        }

        public void SendPreviousFailedEmails()
        {
            foreach (var file in Directory.EnumerateFiles(".", $"*.{_notSendFileExtension}"))
            {
                Mail mailToBeSend;
                using (var fs=File.OpenText(file))
                {
                    mailToBeSend = JsonConvert.DeserializeObject<Mail>(fs.ReadToEnd());
                }

                try
                {
                    _mailSender.SendEmail(mailToBeSend.To, mailToBeSend.Subject, mailToBeSend.Content);
                    File.Delete(file);
                }
                catch {}

            }
        }
    }
}
