using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    /// <summary>
    /// 交通量計算權重
    /// </summary>
    public class TargetWeightAttribute : Attribute
    {
        public double Weight { get; set; }

        public TargetWeightAttribute(double weight)
        {
            Weight = weight;
        }
    }
}