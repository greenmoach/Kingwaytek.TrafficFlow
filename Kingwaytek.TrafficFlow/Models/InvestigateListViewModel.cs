using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public class InvestigateListViewModel
    {
        public int Id { get; set; }

        public InvestigationTypeEnum InvestigationTypeEnum { get; set; }

        public string Town { get; set; }

        public string Intersection { get; set; }

        public DateTime InvestigaionTime { get; set; }

        public DateTime CreateTime { get; set; }
    }
}