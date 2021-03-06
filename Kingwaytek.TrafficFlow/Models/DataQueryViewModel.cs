﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public class DataQueryViewModel
    {
        /// <summary>
        /// 調查類型
        /// </summary>
        public InvestigationQueryTypeEnum QueryType { get; set; }

        /// <summary>
        /// 定位編號
        /// </summary>
        public int PositioningId { get; set; }

        /// <summary>
        /// 調查日期
        /// </summary>
        public DateTime? InvestigaionTime { get; set; }
    }
}