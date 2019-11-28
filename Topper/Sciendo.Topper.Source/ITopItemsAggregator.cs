using System.Collections.Generic;
using Sciendo.Topper.Domain;

namespace Sciendo.Topper.Source
{
    public interface ITopItemsAggregator
    {
        List<TopItem> GetItems(string userName);
        void RegisterProvider(ITopItemsProvider provider);
    }
}