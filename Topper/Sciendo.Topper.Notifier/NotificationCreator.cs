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

        public bool ComposeAndSendMessage(IEnumerable<TopItem> todayItems,IEnumerable<TopItem> yearAggregateItems,string sendTo)
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

        private string ComposeMessage(IEnumerable<TopItem> todayItems, IEnumerable<TopItem> yearAggregateItems)
        {
            DateTime date = DateTime.Today;
            var title = string.Format(Template.Html.TodayItemsTitle, date.Day, date.Month, date.Year);
            StringBuilder message =  new StringBuilder($"{Style.Definition}{title}");
            
            if (todayItems!=null && todayItems.Any())
            {
                var rows = new StringBuilder("");
                foreach (var todayItem in todayItems)
                {
                    rows.Append(string.Format(Template.Html.TodayItemRow, todayItem.Name, todayItem.Hits, todayItem.Score));
                }

                message.Append(string.Format(Template.Html.TodayItemsTable,Style.Today,rows.ToString()));

            }
            else
                message.Append(Template.Html.NoItemsFor7Days);

            if (yearAggregateItems!= null && yearAggregateItems.Any())
            {
                message.Append(Template.Html.YearItemsTitle);
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
                    rows.Append(string.Format(Template.Html.YearItemRow, calculatedStyle,position++,yearAggregatedItem.Name,yearAggregatedItem.Score,yearAggregatedItem.Loved));
                }

                message.Append(string.Format(Template.Html.YearItemsTable, rows.ToString()));
            }
            else
                message.Append(Template.Html.NoItemsForYear);

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
