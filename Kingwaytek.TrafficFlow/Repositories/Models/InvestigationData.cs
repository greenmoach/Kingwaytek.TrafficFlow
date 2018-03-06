using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow.Repositories
{
    public partial class InvestigationData
    {
        public TargetTypeEnum TargetTypeEnum
        {
            get { return (TargetTypeEnum)TargetType; }
            set { TargetType = (int)value; }
        }
    }
}