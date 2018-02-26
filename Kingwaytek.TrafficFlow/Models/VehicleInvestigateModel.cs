using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public class VehicleInvestigateModel
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

        /// <summary>
        /// 大型車
        /// </summary>
        public int[] LargeVehicle { get; set; }

        /// <summary>
        /// 小型車
        /// </summary>
        public int[] LightVehicle { get; set; }

        /// <summary>
        /// 機車
        /// </summary>
        public int[] Motocycle { get; set; }

        /// <summary>
        /// 腳踏車
        /// </summary>
        public int[] Bicycle { get; set; }
    }
}