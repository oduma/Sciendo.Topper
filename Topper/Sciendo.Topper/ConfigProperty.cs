using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigProperty: Attribute
    {
        public string Name { get; set; }

        public ConfigProperty(string name)
        {
            Name = name;
        }
    }
}
