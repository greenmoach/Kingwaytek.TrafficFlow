using System.Data.Entity.Infrastructure;
using System.Web;
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

        [HttpPost]
        public ActionResult UploadInvestigation(HttpPostedFileBase file, InvestigationTypeEnum type)
        {
            var reader = new TRoadReader();
            var models = reader.Convert(file.InputStream);
            return View("CrossRoadPreview", models);
        }
    }
}