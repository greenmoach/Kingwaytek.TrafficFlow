using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingwaytek.TrafficFlow
{
    public interface IIdentity<T>
    {
        T Id { get; set; }
    }
}
