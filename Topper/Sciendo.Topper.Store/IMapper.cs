using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Store
{
    public interface IMapper<TFrom,TTo>
    {
        TTo Map(TFrom input);
    }
}
