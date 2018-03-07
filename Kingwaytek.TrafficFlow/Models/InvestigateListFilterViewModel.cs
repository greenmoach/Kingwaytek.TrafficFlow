using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public class InvestigateListFilterViewModel
    {
        /// <summary>
        /// 行政區
        /// </summary>
        public string Town { get; set; }

        /// <summary>
        /// 調查型態
        /// </summary>
        public InvestigationTypeEnum? Type { get; set; }

        /// <summary>
        /// 建置年度
        /// </summary>
        public int? CreatedYear { get; set; }

        /// <summary>
        /// 關鍵字
        /// </summary>
        public string Keyword { get; set; }
    }
}