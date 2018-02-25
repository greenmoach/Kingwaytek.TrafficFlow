using System;

namespace Kingwaytek.TrafficFlow
{
    /// <summary>
    /// 讀取 .config 設定檔
    /// </summary>
    /// <seealso cref="IConfigProvider" />
    public interface IConfigProvider
    {
        /// <summary>
        /// 讀取 config 中指定鍵值的設定值內容，可帶入第二參數 defaultValue 作為預設值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        string Get(string key, string defaultValue = null);

        /// <summary>
        /// 讀取 config 中指定鍵值的設定值內容，完成轉型傳回指定型別，無預設值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key) where T : IConvertible;

        /// <summary>
        /// 讀取 config 中指定鍵值的設定值內容，完成轉型傳回指定型別，可帶入第二參數 defaultValue 作為預設值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        T Get<T>(string key, T defaultValue) where T : IConvertible;
    }
}