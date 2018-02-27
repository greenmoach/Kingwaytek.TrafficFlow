using System;
using System.Data.Entity.Infrastructure;
using System.Web;
using System.Web.Mvc;
using Kingwaytek.TrafficFlow.Helpers.InvestigationExcelReader;

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
            switch (type)
            {
                case InvestigationTypeEnum.TRoad:
                case InvestigationTypeEnum.Intersection:
                    var intersectionReader = new IntersectionReader();
                    var intersectionModel = intersectionReader.Convert(file.InputStream);
                    return View("CrossRoadPreview", intersectionModel);

                case InvestigationTypeEnum.Pedestrians:
                    var pedestriansReader = new PedestriansReader();
                    var pedestriansModel = pedestriansReader.Convert(file.InputStream);
                    return View("PedestriansPreview", pedestriansModel);

                case InvestigationTypeEnum.FiveWay:
                    var fivewayReader = new FivewayReader();
                    var fivewayModel = fivewayReader.Convert(file.InputStream);
                    return View("FivewayPreview", fivewayModel);

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}