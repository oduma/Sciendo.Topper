using System.Collections.Generic;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;

namespace Sciendo.Topper.Notifier
{
    public interface INotificationManager
    {
        bool ComposeAndSendMessage(List<TopItem> todayItems, List<TopItemWithPartitionKey> yearAggregateItems, string sendTo);
        bool SendPreviousFailedEmails();
    }
}