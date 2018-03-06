using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow.Repositories
{
    public partial class Investigation : IEditColumn, IDeleted
    {
        public InvestigationTypeEnum InvestigationTypeEnum
        {
            get { return (InvestigationTypeEnum)this.InvestigationType; }
            set { this.InvestigationType = (int)value; }
        }
    }
}