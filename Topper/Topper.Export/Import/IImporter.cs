using System;
using System.Collections.Generic;
using System.Text;

namespace Topper.ImportExport.Import
{
    public interface IImporter<T>
    {
        int Import(string inputFileName, ImportTransformation[] importTransformations);
    }
}
