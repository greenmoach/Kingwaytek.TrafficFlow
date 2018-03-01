using Kingwaytek.TrafficFlow.Provider;
using System.Web.Mvc;

namespace Kingwaytek.TrafficFlow.Controllers
{
    public class ControllerBase : Controller
    {
        private IConfigProvider ConfigProviderInstance;

        protected IConfigProvider Config
        {
            get
            {
                if (this.ConfigProviderInstance == null)
                {
                    this.ConfigProviderInstance = new ConfigProvider();
                }

                return this.ConfigProviderInstance;
            }
        }

        /// <summary>
        /// 應用程式資料路徑
        /// </summary>
        public string AppDataPath
        {
            get
            {
                if (HttpContext != null)
                {
                    return HttpContext.Server.MapPath("~/App_Data");
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// 應用程式根目錄
        /// </summary>
        public string AppRootPath
        {
            get
            {
                if (HttpContext != null)
                {
                    return HttpContext.Server.MapPath("~/");
                }

                return string.Empty;
            }
        }
    }
}