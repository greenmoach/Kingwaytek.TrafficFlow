using System.Data.Entity.Infrastructure;
using System.Web.Mvc;

namespace Kingwaytek.TrafficFlow.Controllers
{
    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }
    }
}