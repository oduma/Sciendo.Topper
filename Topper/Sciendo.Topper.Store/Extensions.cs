using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sciendo.Topper.Store
{
    public static class Extensions
    {
        public static string MakeSuitableForId(this string name)
        {
            return name.Replace(@"/", "_").Replace("?","_");
        }
        public static TopItemWithPartitionKey[] AddNewTopItems(this TopItemWithPartitionKey[] initial,
            IEnumerable<TopItem> possibleAdditional,
            IMapper<TopItem,TopItemWithPartitionKey> mapper
            )
        {
            return possibleAdditional
                .Where(p => !initial.Any(i => i.Id == p.Name.MakeSuitableForId()))
                .Select(p =>mapper.Map(p))
                .Union(initial)
                .OrderByDescending(t=>t.Score)
                .ToArray();
        }

        public static DateTime ToDate(this string dateAsString)
        {
            var maxDateParts = dateAsString.Split(new char[] { '-' });
            return new DateTime(Convert.ToInt32(maxDateParts[0]), 
                Convert.ToInt32(maxDateParts[1]), 
                Convert.ToInt32(maxDateParts[2]));

        }
    }
}
