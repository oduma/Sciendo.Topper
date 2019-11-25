using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public interface IMap<TFrom, TTo>
    {
        TTo Map(TFrom fromItem);

    }
}
