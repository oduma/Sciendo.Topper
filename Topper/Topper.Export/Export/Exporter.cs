using Microsoft.Extensions.Logging;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Topper.Export;

namespace Topper.ImportExport.Export
{
    public class Exporter<T> : IExporter<T>
    {
        private readonly ILogger<Exporter<T>> logger;
        private readonly IRepository<T> repository;

        public Exporter(ILogger<Exporter<T>> logger, IRepository<T> repository)
        {
            this.logger = logger;
            this.repository = repository;
        }

        public int Export(string fileName)
        {
            int i = 0;
            using (var fs = File.CreateText(fileName))
            {
                foreach (var topItem in repository.GetAllItemsAsync().Result)
                {
                    if (i == 0)
                        fs.WriteItemHeadLine(topItem);
                    fs.WriteItem(topItem);
                    if (i % 20 == 0)
                        logger.LogInformation("Exported {0} items Ok.", i);
                    i++;
                }
                fs.Flush();
            }
            return i;
        }
    }
}
