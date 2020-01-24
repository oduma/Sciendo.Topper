using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Web
{
    public interface IWebGet<T>
    {
        T Get(Uri url);
    }
}
