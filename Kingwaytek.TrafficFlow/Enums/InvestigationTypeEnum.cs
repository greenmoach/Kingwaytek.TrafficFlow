using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    /// <summary>
    /// 調查型態
    /// </summary>
    public enum InvestigationTypeEnum
    {
        /// <summary>
        /// T字路口車流量
        /// </summary>
        TRoad = 1,

        /// <summary>
        /// 十字路口車流量
        /// </summary>
        Intersection,

        /// <summary>
        /// 行人量調查
        /// </summary>
        Pedestrians,

        /// <summary>
        /// 五叉及五叉以上路口車流量
        /// </summary>
        FiveWay
    }
}
