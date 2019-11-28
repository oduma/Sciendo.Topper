using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Sciendo.Topper.Domain;

namespace Sciendo.Topper.Source
{
    public class TopItemsAggregator : ITopItemsAggregator
    {
        public TopItemsAggregator(ILogger<TopItemsAggregator> logger)
        {
            _providers = new List<ITopItemsProvider>();
            this.logger = logger;
        }
        private List<ITopItemsProvider> _providers;
        private readonly ILogger<TopItemsAggregator> logger;

        public void RegisterProvider(ITopItemsProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            _providers.Add(provider);
            logger.LogInformation("Registered provider {provider}", provider);
        }
        public List<TopItem> GetItems(string userName)
        {
            logger.LogInformation("Getting Top Items...");
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            List<TopItem> result = new List<TopItem>();
            int providersFailed = 0;
            foreach (var provider in _providers)
            {
                var topItems = provider.GetItems(userName);
                if (topItems.Any())
                    if (!result.Any())
                    {
                        result.AddRange(topItems);
                        logger.LogInformation("Top Items added to the pool.");
                    }
                    else
                    {
                        logger.LogInformation("Merging topItems from a different provider...");
                        MergeItems(topItems, result, provider.MergeSourceProperties);
                    }
                else
                {
                    logger.LogWarning("Provider {provider} did not return any items.", provider);
                    providersFailed++;
                }
            }
            if (_providers.Count > providersFailed)
                logger.LogInformation("Retrieved {0} top Items.", result.Count);
            else
                logger.LogWarning("No provider returned results.");
            return result;
        }

        private void MergeItems(List<TopItem> fromList, List<TopItem> toList,
            Action<TopItem, TopItem> mergeSourceProperties)
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
                    logger.LogWarning("No need to merge adding a new top Item.");
                }
            }

        }
    }
}
