using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Sciendo.Config
{
    public class ConfigurationManager<T> where T:class, new()
    {
        public T GetConfiguration(IConfigurationRoot config)
        {
            Log.Information("Getting configuration ...");
            if(config==null)
                throw new ArgumentNullException(nameof(config));
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
            Log.Information("Configuration retrieved: {configClass}", configClass);

            return configClass;
        }
    }
}
