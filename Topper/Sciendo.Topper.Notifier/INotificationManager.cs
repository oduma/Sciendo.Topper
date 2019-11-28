using System.Collections.Generic;
using Sciendo.Topper.Domain;

namespace Sciendo.Topper.Notifier
{
    public interface INotificationManager
    {
        bool ComposeAndSendMessage(List<TopItem> todayItems, List<TopItem> yearAggregateItems, string sendTo);
        bool SendPreviousFailedEmails();
    }
}