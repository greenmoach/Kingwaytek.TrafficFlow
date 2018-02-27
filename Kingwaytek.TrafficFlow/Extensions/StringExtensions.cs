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

        /// <summary>
        /// Get local string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="resourceType"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Try convert string to int, if not return 0
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int ToInt(this string text)
        {
            if (text.IsNullOrEmpty())
            {
                return 0;
            }

            int.TryParse(text, out var value);

            return value;
        }
    }
}