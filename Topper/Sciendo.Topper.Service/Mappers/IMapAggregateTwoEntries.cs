using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public interface IMapAggregateTwoEntries<TFrom, TTo>
    {
        TTo Map(TFrom currentItem, TFrom previousItem);
    }
}
