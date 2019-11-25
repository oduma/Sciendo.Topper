using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public interface IMapAggregateFourEntries<TFrom,TTo>
    {
        TTo Map(TFrom currentDailyItem, TFrom currentOverallItem, TFrom previousDailyItem, TFrom previouslyOverallItem); 
    }
}
