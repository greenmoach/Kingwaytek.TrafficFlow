using System.Web.Mvc;
using Kingwaytek.TrafficFlow.Provider;

namespace Kingwaytek.TrafficFlow
{
    /// <summary>
    /// 表示為了呈現使用 ASP.NET Razor 語法的檢視而必須使用的屬性和方法。
    /// </summary>
    public abstract class WebViewPageBase<T> : WebViewPage<T>
    {
        /// <summary>
        /// Config helper
        /// </summary>
        public IConfigProvider ConfigProvider => new ConfigProvider();
    }
}
