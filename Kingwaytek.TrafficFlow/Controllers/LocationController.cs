using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kingwaytek.TrafficFlow.Controllers
{
    public class LocationController : ControllerBase
    {
        private readonly PositioningService PositioningService;

        private readonly ICacheProvider CacheProvider;

        public LocationController()
        {
            PositioningService = new PositioningService();

            CacheProvider = new MemoryCacheProvider();
        }

        [HttpPost]
        public ActionResult GetRoadsByTown(string town)
        {
            var roads = CacheProvider.Get($"Positioning:Roads:{town}", 24 * 60 * 60, () =>
            {
                var models = PositioningService.GetRoadsByTown(town);

                return models;
            });

            return Json(roads);
        }

        [HttpPost]
        public ActionResult GetRoadsByIntersection(string town, string road)
        {
            var roads = CacheProvider.Get($"Positioning:Roads:{town}{road}", 24 * 60 * 60, () =>
            {
                var models = PositioningService.GetRoadsByIntersection(town, road);

                return models;
            });

            return Json(roads);
        }

        [HttpPost]
        public ActionResult GetPositioning(string town, string road1, string road2)
        {
            var models = PositioningService.GetPositioningByIntersection(town, road1, road2);

            return Json(models);
        }

        [HttpPost]
        public ActionResult GetDirects(int positionId, decimal latitude, decimal longitude, InvestigationTypeEnum type)
        {
            var value = PositioningService.DirectionPositioning(positionId, latitude, longitude, type);
            return Content(value);
        }
    }
}