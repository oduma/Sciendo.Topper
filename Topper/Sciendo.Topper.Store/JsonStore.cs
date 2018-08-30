using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Store
{
    public class JsonStore
    {
        public void Save(string file, TopItem[] topItems)
        {
            using (var fs = File.CreateText(file))
            {

                fs.Write(Newtonsoft.Json.JsonConvert.SerializeObject(topItems));

            }
        }
    }
}
