using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Notifier
{
    public class NotificationManager
    {
        private readonly IEmailSender _mailSender;
        private readonly string _notSendFileExtension;

        public NotificationManager(IEmailSender mailSender, string notSendFileExtension="mail")
        {
            _mailSender = mailSender ?? throw new ArgumentNullException(nameof(mailSender));
            _notSendFileExtension = notSendFileExtension;
        }

        public bool ComposeAndSendMessage(List<TopItem> todayItems,
            List<TopItem> yearAggregateItems,
            string sendTo)
        {
            if(string.IsNullOrEmpty(sendTo))
                return false;

            var mailToBeSent = new Mail
                {To = sendTo, Subject = Template.Subject, Content = ComposeMessage(todayItems, yearAggregateItems)};
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
            mailToBeSent.DateTime=DateTime.Now;
            var fileName = $"{mailToBeSent.DateTime}{mailToBeSent.To}.{_notSendFileExtension}";
            using (var fs = File.CreateText(fileName))
            {
                fs.Write(JsonConvert.SerializeObject(mailToBeSent));
            }
        }

        private string ComposeMessage(List<TopItem> todayItems, List<TopItem> yearAggregateItems)
        {
            var title = string.Format(Template.Html.TodayItemsTitle, DateTime.Today.Day, DateTime.Today.Month,
                DateTime.Today.Year);

            StringBuilder message =  new StringBuilder($"{Style.Definition}{title}");
            
            if (todayItems!=null && todayItems.Any())
            {
                var rows = new StringBuilder("");
                foreach (var todayItem in todayItems)
                {
                    rows.Append(string.Format(Template.Html.TodayItemRow, todayItem.Name, todayItem.Hits, todayItem.Score));
                }

                message.Append(string.Format(Template.Html.TodayItemsTable,Style.Today,rows));

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
                    if (todayItems!=null && todayItems.Any((t) => t.Name == yearAggregatedItem.Name))
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

                message.Append(string.Format(Template.Html.YearItemsTable, rows));
            }
            else
                message.Append(Template.Html.NoItemsForYear);

            return message.ToString();
        }

        public bool SendPreviousFailedEmails()
        {
            bool sentForToday = false;
            foreach (var file in Directory.EnumerateFiles(".", $"*.{_notSendFileExtension}"))
            {
                Mail mailToBeSend;
                using (var fs=File.OpenText(file))
                {
                    mailToBeSend = JsonConvert.DeserializeObject<Mail>(fs.ReadToEnd());
                    if (mailToBeSend.DateTime.HasValue && mailToBeSend.DateTime.Value.Day == DateTime.Now.Day &&
                        mailToBeSend.DateTime.Value.Month == DateTime.Now.Month &&
                        mailToBeSend.DateTime.Value.Year == DateTime.Now.Year)
                    {
                        sentForToday = true;
                    }
                }

                try
                {
                    _mailSender.SendEmail(mailToBeSend.To, mailToBeSend.Subject, mailToBeSend.Content);
                    File.Delete(file);
                }
                catch
                {
                    // if the email failed to send for whatever reason don't stop
                    //will retry next time
                }
            }

            return sentForToday;
        }
    }
}
