using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Topper.Export
{
    public static class Extensions
    {
        public static void WriteItemHeadLine<T>(this StreamWriter streamWriter, T currentItem)
        {
            var propertyNames = string.Join(',', currentItem.GetType().GetProperties().Select(p => p.Name));
            streamWriter.WriteLine(propertyNames);
        }

        public static void WriteItem<T>(this StreamWriter streamWriter, T currentItem)
        {
            var propertyValues = string.Join(',', currentItem.GetType().GetProperties().Select(p => p.GetValue(currentItem).ToString()));
            streamWriter.WriteLine(propertyValues);
        }
    }
}
