using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    /// <summary>
    /// 調查目標列舉
    /// </summary>
    public enum TargetTypeEnum
    {
        /// <summary>
        /// 行人
        /// </summary>
        Pedestrians = 1,

        /// <summary>
        /// 大型車
        /// </summary>
        [TargetWeight(2)]
        LargeVehicle,

        /// <summary>
        /// 小型車
        /// </summary>
        [TargetWeight(1)]
        LightVehicle,

        /// <summary>
        /// 機車
        /// </summary>
        [TargetWeight(0.3)]
        Motocycle,

        /// <summary>
        /// 腳踏車
        /// </summary>
        [TargetWeight(0.3)]
        Bicycle
    }
}