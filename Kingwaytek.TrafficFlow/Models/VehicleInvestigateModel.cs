using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public class VehicleInvestigateModel : BasicDescriptionModel
    {
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