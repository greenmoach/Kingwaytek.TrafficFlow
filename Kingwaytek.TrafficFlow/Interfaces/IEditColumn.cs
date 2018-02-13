using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public interface IEditColumn
    {
        System.DateTime CreateTime { get; set; }

        int CreateUserId { get; set; }

        System.DateTime? LastEditTime { get; set; }

        int? LastEditUserId { get; set; }
    }
}
