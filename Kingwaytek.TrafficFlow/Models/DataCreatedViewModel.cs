using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public class DataCreatedViewModel : GeometryViewModel
    {
        /// <summary>
        /// 調查型態
        /// </summary>
        public InvestigationTypeEnum Type { get; set; }

        /// <summary>
        /// 定位編號
        /// </summary>
        public int PositioningId { get; set; }

        /// <summary>
        /// 定位城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 定位鄉鎮
        /// </summary>
        public string Town { get; set; }

        /// <summary>
        /// 定位交叉路口1
        /// </summary>
        public string Road1 { get; set; }

        /// <summary>
        /// 定位交叉路口2
        /// </summary>
        public string Road2 { get; set; }

        /// <summary>
        /// 路口指向設定
        /// </summary>
        public string Positioning { get; set; }

        /// <summary>
        /// 調查日期
        /// </summary>
        public DateTime InvestigaionTime { get; set; }

        /// <summary>
        /// 路口管制說明
        /// </summary>
        public string TrafficControlNote { get; set; }

        /// <summary>
        /// 天候
        /// </summary>
        public string Weather { get; set; }

        /// <summary>
        /// 匯入的調查檔案識別編號
        /// </summary>
        public string FileIdentification { get; set; }

        /// <summary>
        /// 站號
        /// </summary>
        public string IntersectionId { get; set; }
    }
}