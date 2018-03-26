using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow.Models
{
    public class DirectViewModel
    {
        public DirectPositionViewModel Center { get; set; }

        public IEnumerable<DirectPositionViewModel> Directions { get; set; }
    }
}