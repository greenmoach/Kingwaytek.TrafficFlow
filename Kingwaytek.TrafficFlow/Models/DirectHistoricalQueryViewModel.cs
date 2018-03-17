using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public class DirectHistoricalQueryViewModel
    {
        /// <summary>
        /// 調查路口
        /// </summary>
        public string Intersection { get; set; }

        /// <summary>
        /// 小時區間名稱
        /// </summary>
        public string HourlyInterval { get; set; }

        /// <summary>
        /// 定位編號
        /// </summary>
        public int PositioningId { get; set; }
    }
}