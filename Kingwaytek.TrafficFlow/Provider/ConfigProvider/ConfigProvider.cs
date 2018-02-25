using System;
using System.Configuration;

namespace Kingwaytek.TrafficFlow.Provider
{
    public class ConfigProvider : IConfigProvider
    {
        public virtual string Get(string key, string defaultValue = null)
        {
            return ConfigurationManager.AppSettings[key] ?? defaultValue;
        }

        public T Get<T>(string key) where T : IConvertible
        {
            var value = this.Get(key);

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public T Get<T>(string key, T defaultValue) where T : IConvertible
        {
            var value = this.Get(key);

            if (value.IsNullOrEmpty())
            {
                return defaultValue;
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}