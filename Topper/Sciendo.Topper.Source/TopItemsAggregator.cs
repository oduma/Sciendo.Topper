using System;
using System.Collections.Generic;
using System.Linq;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Source
{
    public class TopItemsAggregator
    {
        public TopItemsAggregator()
        {
            _providers=new List<ITopItemsProvider>();
        }
        private List<ITopItemsProvider> _providers;
        public void RegisterProvider(ITopItemsProvider provider)
        {
            if(provider == null)
                throw new ArgumentNullException(nameof(provider));
            _providers.Add(provider);
        }
        public List<TopItem> GetItems(string userName)
        {
            if(string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            List<TopItem> result = new List<TopItem>();
            foreach (var provider in _providers)
            {
                var topItems= provider.GetItems(userName);
                if (!result.Any())
                {
                    result.AddRange(topItems);
                }
                else
                {
                    MergeItems(topItems, result, provider.MergeSourceProperties);
                }
            }
            return result;
        }

        private void MergeItems(List<TopItem> fromList, List<TopItem> toList, 
            Action<TopItem,TopItem> mergeSourceProperties)
        {
            foreach (var fromTopItem in fromList)
            {
                var toTopItem = toList.FirstOrDefault(t => t.Name == fromTopItem.Name);

                if (toTopItem != null)
                {
                    mergeSourceProperties(fromTopItem, toTopItem);
                }
                else
                {
                    toList.Add(fromTopItem);
                }
            }

        }
    }
}
