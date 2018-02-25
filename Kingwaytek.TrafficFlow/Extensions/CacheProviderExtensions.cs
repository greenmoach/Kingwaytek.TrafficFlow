using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    /// <summary>
    /// 根據 ICacheProvider 的 Cache-Aside pattern
    /// </summary>
    public static class CacheProviderExtensions
    {
        /// <summary>
        /// 取得或依據委派初始化快取內容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheProvider"></param>
        /// <param name="key"></param>
        /// <param name="acquire"></param>
        /// <returns></returns>
        public static T Get<T>(this ICacheProvider cacheProvider, string key, Func<T> acquire)
        {
            return Get(cacheProvider, key, 60, acquire);
        }

        /// <summary>
        /// 取得或依據委派初始化快取內容, 設定的快取內容存留時間參考 cacheSeconds
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheProvider"></param>
        /// <param name="key"></param>
        /// <param name="cacheSeconds"></param>
        /// <param name="acquire"></param>
        /// <returns></returns>
        public static T Get<T>(this ICacheProvider cacheProvider, string key, int cacheSeconds, Func<T> acquire)
        {
            if (cacheProvider.IsSet(key))
            {
                return cacheProvider.Get<T>(key);
            }

            var result = acquire();

            cacheProvider.Set(key, result, cacheSeconds);

            return result;
        }
    }
}