using System;
using System.Collections.Generic;
using System.Linq;
using Sciendo.Topper.Contracts;
using Serilog;

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
            Log.Information("Registered provider {provider}", provider);
        }
        public List<TopItem> GetItems(string userName)
        {
            Log.Information("Getting Top Items...");
            if(string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            List<TopItem> result = new List<TopItem>();
            int providersFailed = 0;
            foreach (var provider in _providers)
            {
                var topItems= provider.GetItems(userName);
                if(topItems.Any())
                    if (!result.Any())
                {
                    result.AddRange(topItems);
                    Log.Information("Top Items added to the pool.");
                }
                else
                {
                    Log.Information("Merging topItems from a different provider...");
                    MergeItems(topItems, result, provider.MergeSourceProperties);
                }
                else
                {
                    Log.Warning("Provider {provider} did not return any items.",provider);
                    providersFailed++;
                }
            }
            if(_providers.Count>providersFailed)
                Log.Information("Retrieved {0} top Items.", result.Count);
            else
                Log.Warning("No provider returned results.");
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
                    Log.Warning("No need to merge adding a new top Item.");
                }
            }

        }
    }
}
