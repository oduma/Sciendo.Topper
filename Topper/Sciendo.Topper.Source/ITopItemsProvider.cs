using Sciendo.Topper.Domain;
using System.Collections.Generic;

namespace Sciendo.Topper.Source
{
    public interface ITopItemsProvider
    {
        List<TopItem> GetItems(string userName);
        void MergeSourceProperties(TopItem fromItem, TopItem toItem);
    }
}
