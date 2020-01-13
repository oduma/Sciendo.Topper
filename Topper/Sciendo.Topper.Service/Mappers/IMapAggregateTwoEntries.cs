using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public interface IMapAggregateTwoEntries<TFromMain, TFrom, TTo>
    {
        TTo Map(TFromMain currentItem, TFrom previousItem);
    }
}
