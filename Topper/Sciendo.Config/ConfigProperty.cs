using System;

namespace Sciendo.Config
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
