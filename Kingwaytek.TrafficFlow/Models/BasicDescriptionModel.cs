using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public class BasicDescriptionModel
    {
        /// <summary>
        /// 調查開始時間
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 調查結束時間
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 指向
        /// </summary>
        public string DirectionCode { get; set; }
    }
}