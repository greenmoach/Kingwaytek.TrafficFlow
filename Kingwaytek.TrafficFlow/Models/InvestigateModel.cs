using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public class InvestigateModel<T>
    {
        /// <summary>
        /// 天候
        /// </summary>
        public string Weather { get; set; }

        /// <summary>
        /// 調查(計數)資料
        /// </summary>
        public List<T> Data { get; set; } = new List<T>();
    }
}