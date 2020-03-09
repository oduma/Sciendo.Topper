using System;
using System.Collections.Generic;
using System.Text;

namespace Topper.ImportExport.Export
{
    public interface IExporter<T>
    {
        int Export(string fileName);
    }
}
