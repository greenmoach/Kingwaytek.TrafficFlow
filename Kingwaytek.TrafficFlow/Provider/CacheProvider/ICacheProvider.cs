using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    /// <summary>
    /// 快取介面
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// 取得快取內容
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        T Get<T>(string key);

        /// <summary>
        /// 指定快取內容
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheSeconds">Cache time</param>
        void Set(string key, object data, int cacheSeconds);

        /// <summary>
        /// 確定快取鍵值是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        bool IsSet(string key);

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// 移除快取內容
        /// </summary>
        /// <param name="key">/key</param>
        void Remove(string key);

        /// <summary>
        /// 依據鍵值樣式移除快取內容
        /// </summary>
        /// <param name="pattern">pattern</param>
        void RemoveByPattern(string pattern);

        /// <summary>
        /// 清除所有快取
        /// </summary>
        void Clear();

        /// <summary>
        /// 數值自動遞增
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cacheSeconds">Cache 秒數</param>
        /// <param name="startValue">起始值</param>
        long Increment(string key, int cacheSeconds, long startValue = 1);
    }
}