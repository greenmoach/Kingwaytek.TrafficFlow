using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kingwaytek.TrafficFlow.Provider;

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
