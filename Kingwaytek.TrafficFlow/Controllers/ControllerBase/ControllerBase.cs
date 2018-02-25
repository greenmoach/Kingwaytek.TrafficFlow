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
    }
}