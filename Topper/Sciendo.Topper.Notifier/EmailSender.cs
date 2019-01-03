using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Serilog;

namespace Sciendo.Topper.Notifier
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfig _senderConfig;

        public EmailSender(EmailConfig senderConfig)
        {
            if(senderConfig==null || string.IsNullOrEmpty(senderConfig.Domain))
                throw new ArgumentNullException(nameof(senderConfig));
            _senderConfig = senderConfig;
        }

        public Task SendEmailAsync(string toEmail, string subject, string message)
        {
            if(string.IsNullOrEmpty(toEmail))
                throw new ArgumentNullException(nameof(toEmail));

            MailMessage mail = GetMailMessage(toEmail, subject, message,
                _senderConfig.DefaultSenderEmail, _senderConfig.DefaultSenderDisplayName, _senderConfig.UseHtml);
            SmtpClient client = GetSmtpClient(_senderConfig.Domain, _senderConfig.Port, _senderConfig.RequiresAuthentication,
                _senderConfig.UserName, _senderConfig.Key, _senderConfig.UseSsl);

            return client.SendMailAsync(mail);
        }

        public void SendEmail(string toEmail, string subject, string message)
        {
            Log.Information("Sending email...");
            if(string.IsNullOrEmpty(toEmail))
                throw new ArgumentNullException(nameof(toEmail));
            
            MailMessage mail = GetMailMessage(toEmail, subject, message,
                _senderConfig.DefaultSenderEmail, _senderConfig.DefaultSenderDisplayName, _senderConfig.UseHtml);
            SmtpClient client = GetSmtpClient(_senderConfig.Domain, _senderConfig.Port, _senderConfig.RequiresAuthentication,
                _senderConfig.UserName, _senderConfig.Key, _senderConfig.UseSsl);

            client.Send(mail);
            Log.Information("Email sent.");
        }

        private static MailMessage GetMailMessage(string toEmail, string subject, string message,
            string defaultSenderEmail, string defaultSenderDisplayName = null, bool useHtml = true)
        {
            Log.Information("Composing email...");
            MailAddress sender;

            if (string.IsNullOrEmpty(defaultSenderEmail))
            {
                throw new ArgumentException("No sender mail address was provided");
            }
            else
            {
                sender = !string.IsNullOrEmpty(defaultSenderDisplayName) ?
                    new MailAddress(defaultSenderEmail, defaultSenderDisplayName) : new MailAddress(defaultSenderEmail);
            }

            MailMessage mail = new MailMessage()
            {
                From = sender,
                Subject = subject,
                Body = message,
                IsBodyHtml = useHtml
            };
            mail.To.Add(toEmail);
            Log.Information("Mail composed.");
            return mail;
        }

        private static SmtpClient GetSmtpClient(string host, int port, bool requiresAuthentication = true,
            string userName = null, string userKey = null, bool useSsl = false)
        {
            Log.Information("Building an email client...");
            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentException("No domain was provided");
            }
            SmtpClient client = new SmtpClient(host);

            if (port > -1)
            {
                client.Port = port;
            }

            client.UseDefaultCredentials = !requiresAuthentication;

            if (requiresAuthentication)
            {
                if (string.IsNullOrEmpty(userName))
                {
                    throw new ArgumentException("No user name was provided");
                }

                client.Credentials = new NetworkCredential(userName, userKey);
            }

            client.EnableSsl = useSsl;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            Log.Information("Email client built.");
            return client;
        }
    }
}
