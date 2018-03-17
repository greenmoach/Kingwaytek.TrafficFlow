using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public class DirectHistoricalViewModel
    {
        /// <summary>
        /// 調查時間
        /// </summary>
        public string InvestigaionTime { get; set; }

        /// <summary>
        /// 轉向資料
        /// </summary>
        public Dictionary<string, int> Directions { get; set; }
    }
}