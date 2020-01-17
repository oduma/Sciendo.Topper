using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemsToTimeInterval : IMapAggregateTwoEntries<TopItem, TopItem, TimeInterval>
    {
        public TimeInterval Map(TopItem minItem, TopItem maxItem)
        {
            if (minItem == null)
                throw new ArgumentNullException(nameof(minItem));
            if (maxItem == null)
                throw new ArgumentNullException(nameof(maxItem));
            return new TimeInterval { 
                FromDate = minItem.Date.ToString("yyyy-MM-dd"), 
                ToDate = maxItem.Date.ToString("yyyy-MM-dd") };
        }
    }
}
