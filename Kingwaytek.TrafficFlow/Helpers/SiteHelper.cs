using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public static class SiteHelper
    {
        private static ICacheProvider CacheProvider => new MemoryCacheProvider();

        public static List<string> GetTowns()
        {
            var towns = CacheProvider.Get("Positioning:Town:List", 24 * 60 * 60, () =>
            {
                var service = new PositioningService();
                var models = service.GetTowns()
                                    .Select(x => x.TownName)
                                    .ToList();

                return models;
            });

            return towns;
        }

        public static List<int> GetInvestigationYears()
        {
            var years = CacheProvider.Get("Investigation:Year:List", 24 * 60 * 60, () =>
            {
                var service = new InvestigateService();
                var models = service.GetAllInvestigationYears();

                return models;
            });

            return years;
        }
    }
}