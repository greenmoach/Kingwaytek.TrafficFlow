using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public class TrafficViewModel
    {
        /// <summary>
        /// 指向
        /// </summary>
        public string Intersection { get; set; }

        /// <summary>
        /// 轉向
        /// </summary>
        public string Direction { get; set; }

        /// <summary>
        /// 統計量
        /// </summary>
        public int Amount { get; set; }
    }
}