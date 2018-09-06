using System.Threading.Tasks;

namespace Sciendo.Topper.Notifier
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        void SendEmail(string email, string subject, string message);
    }
}
