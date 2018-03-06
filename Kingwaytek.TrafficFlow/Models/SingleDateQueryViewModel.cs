using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public class SingleDateQueryViewModel
    {
        /// <summary>
        /// 調查型態
        /// </summary>
        public InvestigationTypeEnum InvestigationType { get; set; }

        /// <summary>
        /// 調查時間
        /// </summary>
        public DateTime InvestigaionTime { get; set; }

        /// <summary>
        /// 其他可查詢的調查時間
        /// </summary>
        public IEnumerable<DateTime> OtherInvestigaionTime { get; set; }

        /// <summary>
        /// 小時區間資料
        /// </summary>
        public IEnumerable<SingleDateHourlyViewModel> HourlyIntervals { get; set; }

        /// <summary>
        /// 站號
        /// </summary>
        public string IntersectionId { get; set; }

        /// <summary>
        /// 站名
        /// </summary>
        public string IntersectionName { get; set; }

        /// <summary>
        /// 天候
        /// </summary>
        public string Weather { get; set; }

        /// <summary>
        /// 匯入的調查檔案識別編號
        /// </summary>
        public string FileIdentification { get; set; }

        /// <summary>
        /// 路口定位轉向地理資訊
        /// </summary>
        public string Positioning { get; set; }

        /// <summary>
        /// 路口管制說明
        /// </summary>
        public string TrafficControlNote { get; set; }
    }
}