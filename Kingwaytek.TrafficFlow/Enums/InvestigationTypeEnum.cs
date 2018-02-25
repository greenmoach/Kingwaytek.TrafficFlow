using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "T字路口車流量")]
        TRoad = 1,

        /// <summary>
        /// 十字路口車流量
        /// </summary>
        [Display(Name = "十字路口車流量")]
        Intersection,

        /// <summary>
        /// 行人量調查
        /// </summary>
        [Display(Name = "行人量調查")]
        Pedestrians,

        /// <summary>
        /// 五叉及五叉以上路口車流量
        /// </summary>
        [Display(Name = "五叉及五叉以上路口車流量")]
        FiveWay
    }
}