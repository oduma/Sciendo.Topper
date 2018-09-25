using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Sciendo.Topper
{
    public class ConfigurationManager<T> where T:class, new()
    {
        public T GetConfiguration(IConfigurationRoot config)
        {
            T configClass = new T();
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                foreach (var attribute in property.GetCustomAttributes<ConfigProperty>())
                {
                    var instanceProperty = configClass.GetType().GetProperty(property.Name,
                        BindingFlags.Instance | BindingFlags.Public);
                    if (instanceProperty != null && instanceProperty.CanWrite)
                    {
                        instanceProperty.SetValue(configClass,
                            config.GetSection(attribute.Name).Get(property.PropertyType));
                    }
                    else
                    {
                        throw new Exception("configuration exception");
                    }
                }
            }

            return configClass;
        }
    }
}
