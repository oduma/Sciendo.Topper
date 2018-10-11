using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Source
{
    public class TopItemsSourcer
    {
        public TopItemsSourcer()
        {
            _sourcersWithResults=new List<ILastFmSourcer>();
        }
        private List<ILastFmSourcer> _sourcersWithResults;
        public void RegisterSourcer(ILastFmSourcer sourcer)
        {
            _sourcersWithResults.Add(sourcer);
        }
        public List<TopItem> GetItems(string userName)
        {
            List<TopItem> result = new List<TopItem>();
            foreach (var sourcer in _sourcersWithResults)
            {
                var sourcerTopItems= sourcer.GetItems(userName);
                if (!result.Any())
                {
                    result.AddRange(sourcerTopItems);
                }
                else
                {
                    MergeItems(sourcerTopItems, result, sourcer.MergeSourceProperties);
                }
            }
            return result;
        }

        private void MergeItems(List<TopItem> fromList, List<TopItem> toList, 
            Action<TopItem,TopItem> mergeSourcerProperties)
        {
            foreach (var fromTopItem in fromList)
            {
                var toTopItem = toList.FirstOrDefault(t => t.Name == fromTopItem.Name);

                if (toTopItem != null)
                {
                    mergeSourcerProperties(fromTopItem, toTopItem);
                }
                else
                {
                    toList.Add(fromTopItem);
                }
            }

        }
    }
}
