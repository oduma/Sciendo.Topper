using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public interface IMapAggregateFourEntries<TFromMain, TFrom,TTo>
    {
        TTo Map(TFromMain currentDailyItem, TFrom currentOverallItem, TFrom previousDailyItem, TFrom previouslyOverallItem); 
    }
}
