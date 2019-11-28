using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sciendo.Topper.Domain;

namespace Sciendo.Topper.Notifier
{
    public class NotificationManager : INotificationManager
    {
        private readonly IEmailSender _mailSender;
        private readonly string _notSendFileExtension;
        private readonly ILogger<NotificationManager> logger;

        public NotificationManager(ILogger<NotificationManager> logger, 
            IEmailSender mailSender, string notSendFileExtension = "mail")
        {
            this.logger = logger;
            _mailSender = mailSender ?? throw new ArgumentNullException(nameof(mailSender));
            _notSendFileExtension = notSendFileExtension;
        }

        public bool ComposeAndSendMessage(List<TopItem> todayItems,
            List<TopItem> yearAggregateItems,
            string sendTo)
        {
            logger.LogInformation("Composing and Sending email...");
            if (string.IsNullOrEmpty(sendTo))
                return false;

            var mailToBeSent = new Mail
            { To = sendTo, Subject = Template.Subject, Content = ComposeMessage(todayItems, yearAggregateItems) };
            try
            {
                _mailSender.SendEmail(mailToBeSent.To, mailToBeSent.Subject, mailToBeSent.Content);
                return true;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Email not sent.");
                PersistLocallyFile(mailToBeSent);
                return false;
            }
        }

        private void PersistLocallyFile(Mail mailToBeSent)
        {
            logger.LogInformation("Saving email to the file system for later...");
            mailToBeSent.DateTime = DateTime.Now;
            var fileName = $"{mailToBeSent.DateTime.Value.Ticks}{mailToBeSent.To}.{_notSendFileExtension}";
            using (var fs = File.CreateText(fileName))
            {
                fs.Write(JsonConvert.SerializeObject(mailToBeSent));
            }
            logger.LogInformation("Email saved to file {fileName}", fileName);
        }

        private string ComposeMessage(List<TopItem> todayItems, List<TopItem> yearAggregateItems)
        {
            logger.LogInformation("Composing the body of message...");
            var title = string.Format(Template.Html.TodayItemsTitle, DateTime.Today.Day, DateTime.Today.Month,
                DateTime.Today.Year);

            StringBuilder message = new StringBuilder($"{Style.Definition}{title}");

            if (todayItems != null && todayItems.Any())
            {
                var rows = new StringBuilder("");
                foreach (var todayItem in todayItems)
                {
                    rows.Append(string.Format(Template.Html.TodayItemRow, todayItem.Name, todayItem.Hits, todayItem.Score));
                }

                message.Append(string.Format(Template.Html.TodayItemsTable, Style.Today, rows));
                logger.LogInformation("Added today's items to the body.");
            }
            else
            {
                logger.LogInformation("No Items for today.");
                message.Append(Template.Html.NoItemsFor7Days);
            }

            if (yearAggregateItems != null && yearAggregateItems.Any())
            {
                message.Append(Template.Html.YearItemsTitle);
                var rows = new StringBuilder("");
                int position = 1;
                foreach (var yearAggregatedItem in yearAggregateItems)
                {
                    var calculatedStyle = "";
                    if (todayItems != null && todayItems.Any((t) => t.Name == yearAggregatedItem.Name))
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
                    rows.Append(string.Format(Template.Html.YearItemRow, calculatedStyle, position++, yearAggregatedItem.Name, yearAggregatedItem.Score, yearAggregatedItem.Loved));
                }

                message.Append(string.Format(Template.Html.YearItemsTable, rows));
                logger.LogInformation("Added Year's Aggregate items.");
            }
            else
            {
                logger.LogInformation("No items for the year.");
                message.Append(Template.Html.NoItemsForYear);
            }

            return message.ToString();
        }

        public bool SendPreviousFailedEmails()
        {
            logger.LogInformation("Trying to send previous emails...");
            bool sentForToday = false;
            int sent = 0;
            int total = 0;
            foreach (var file in Directory.EnumerateFiles(".", $"*.{_notSendFileExtension}"))
            {
                logger.LogInformation("Found previous un-send email: {file}.", file);
                Mail mailToBeSend;
                total++;
                using (var fs = File.OpenText(file))
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
                    logger.LogInformation("Trying to resend email from:{file}", file);
                    _mailSender.SendEmail(mailToBeSend.To, mailToBeSend.Subject, mailToBeSend.Content);
                    File.Delete(file);
                    sent++;
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Cannot send or delete email from file {file}", file);
                    // if the email failed to send for whatever reason don't stop
                    //will retry next time
                }
            }
            if (total == sent && total == 0)
                logger.LogInformation("No previous emails to send.");
            else
                logger.LogInformation("Successfully sent {sent} emails from {total}", sent, total);
            return sentForToday;
        }
    }
}
