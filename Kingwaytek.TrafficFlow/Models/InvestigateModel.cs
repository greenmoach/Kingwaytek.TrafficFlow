using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public class InvestigateModel<T> : IInvestigateModel<T>
    {
        /// <summary>
        /// 站號
        /// </summary>
        public string IntersectionId { get; set; }

        /// <summary>
        /// 天候
        /// </summary>
        public string Weather { get; set; }

        /// <summary>
        /// 調查(計數)資料
        /// </summary>
        public List<T> Data { get; set; } = new List<T>();

        /// <summary>
        /// 匯入的調查檔案識別編號
        /// </summary>
        public string FileIdentification { get; set; }

        /// <summary>
        /// 匯入的資料是否與既有的資料有重複
        /// </summary>
        public bool HasOverlayData { get; set; }
    }
}