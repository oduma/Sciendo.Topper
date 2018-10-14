using System;
using System.Collections.Generic;
using System.Text;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Source
{
    public interface ITopItemsProvider
    {
        List<TopItem> GetItems(string userName);
        void MergeSourceProperties(TopItem fromItem, TopItem toItem);
    }
}
