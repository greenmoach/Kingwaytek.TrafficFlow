using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;

namespace Kingwaytek.TrafficFlow
{
    /// <summary>
    /// JsonExtensions
    /// </summary>
    public static class JsonExtensions
    {
        public static string ToJson(this object target, bool IsCamelCase = true)
        {
            var setting = new JsonSerializerSettings()
            {
                ContractResolver = IsCamelCase ? new CamelCasePropertyNamesContractResolver() : new DefaultContractResolver()
            };

            return JsonConvert.SerializeObject(target, Formatting.None, setting);
        }

        public static T ToTypedObject<T>(this string s)
        {
            if (s.IsNullOrEmpty() || !Regex.IsMatch(s, @"^(\[|\{)(.|\n)*(\]|\})$", RegexOptions.Compiled))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(s);
        }
    }
}