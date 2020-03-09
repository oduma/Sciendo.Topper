using Microsoft.Extensions.Logging;
using Sciendo.Topper.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Topper.ImportExport.Import
{
    public class Importer<T> : IImporter<T>
    {
        private readonly ILogger<Importer<T>> logger;
        private readonly IRepository<T> repository;

        public Importer(ILogger<Importer<T>> logger, IRepository<T> repository)
        {
            this.logger = logger;
            this.repository = repository;
        }
        public int Import(string inputFileName, ImportTransformation[] imporTransformations)
        {
            IEnumerable<T> dataToMigrate;
            try
            {
                dataToMigrate = ReadFile(inputFileName, imporTransformations);
            }
            catch (Exception e)
            {
                logger.LogError(e, "");
                return 0;
            }
            return UpdateItems(dataToMigrate);
        }

        private int UpdateItems(IEnumerable<T> dataToMigrate)
        {
            foreach(var item in dataToMigrate)
            {
                repository.UpsertItemAsync(item).Wait();
            }
            return dataToMigrate.Count();
        }

        private IEnumerable<T> ReadFile(string inputFileName, ImportTransformation[] importTransformations)
        {
            if (!File.Exists(inputFileName))
                throw new IOException($"{inputFileName} does not exist.");
            logger.LogInformation("Reading file: {0}", inputFileName);
            bool isFirstLine = true;
            Dictionary<int, string> orderedPropertyNames = new Dictionary<int, string>();
            foreach (var fileLine in File.ReadLines(inputFileName))
            {
                if(isFirstLine)
                {
                    orderedPropertyNames= GetOrderedPropertyNames(fileLine);
                    isFirstLine = false;
                }
                else
                {
                    yield return
                        GetItemFromLine(fileLine.Split(new char[] { ',' }, StringSplitOptions.None), orderedPropertyNames, importTransformations);
                }
            }
        }

        private T GetItemFromLine(string[] item, Dictionary<int, string> orderedPropertyNames, ImportTransformation[] importTransformations)
        {
            T newItem = Activator.CreateInstance<T>();
            for(int i=0;i<item.Length;i++)
            {
                var propertyInfo = typeof(T).GetProperty(orderedPropertyNames[i]);
                if(propertyInfo==null)
                {
                    propertyInfo = typeof(T).GetProperty(importTransformations.Where(t => t.SourcePropertyNames.Contains(orderedPropertyNames[i])).FirstOrDefault().TargetPropertyName);
                    if (propertyInfo == null)
                        throw new Exception($"Missing Transformation for source: {orderedPropertyNames[i]}");
                }
                var propertyType = propertyInfo.PropertyType;
                
                propertyInfo.SetValue(newItem, GetObjectValue(item[i],propertyType));
            }
            return newItem;
        }

        private object GetObjectValue(string value, Type propertyType)
        {
            if (propertyType.Name == "String")
                return value;
            if (propertyType.Name == "Int32")
                return Convert.ToInt32(value);
            if(propertyType.Name=="DateTime")
            {
                var valueParts = value.Split(new char[] { '/' });
                var day = Convert.ToInt32(valueParts[0]);
                var month = Convert.ToInt32(valueParts[1]);
                var yearParts = valueParts[2].Split(new char[] { ' ' });
                var year = Convert.ToInt32(yearParts[0]);
                return new DateTime(year, month, day);
            }
            return null;
        }

        private Dictionary<int, string> GetOrderedPropertyNames(string fileLine)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            var fileLineParts = fileLine.Split(new char[] { ',' });
            for(int i=0;i<fileLineParts.Length;i++)
            {
                result.Add(i, fileLineParts[i]);
            }
            return result;
        }
    }
}
