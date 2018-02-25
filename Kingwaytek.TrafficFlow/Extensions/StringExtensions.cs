using System;
using System.Resources;

namespace Kingwaytek.TrafficFlow
{
    public static class StringExtensions
    {
        /// <summary>
        /// 判斷字串是否為空值
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        public static string GetLocalString(this string source, Type resourceType)
        {
            var resourceManager = new ResourceManager(resourceType);
            var localString = resourceManager.GetString(source);
            if (localString != string.Empty)
            {
                return localString;
            }

            return source;
        }
    }
}